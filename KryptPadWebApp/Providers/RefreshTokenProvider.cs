using KryptPadWebApp.Cryptography;
using KryptPadWebApp.Models;
using KryptPadWebApp.Models.Entities;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Infrastructure;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace KryptPadWebApp.Providers
{
    public class RefreshTokenProvider : IAuthenticationTokenProvider
    {

        public async Task CreateAsync(AuthenticationTokenCreateContext context)
        {
            // Generate token
            var token = Encryption.GenerateRandomString(32);
            // Get the client id
            var clientId = context.Ticket.Properties.Dictionary["as:client_id"];

            // Store the refresh token in the database
            using (var ctx = new ApplicationDbContext())
            {
                // Get the username
                var username = context.Ticket.Identity.Name;

                // Find any existing refresh tokens for the user/client and remove them
                var existingTokens = ctx.RefreshTokens.Where((x) => x.ClientId == clientId && x.Username == username);

                // Delete the existing tokens
                ctx.RefreshTokens.RemoveRange(existingTokens);

                // Get the access token time to live (ttl)
                var refreshTokenTTL = Convert.ToInt32(ConfigurationManager.AppSettings["RefreshTokenTTL"]);

                // Create refresh token
                var rt = new RefreshToken()
                {
                    Id = Encryption.Hash(token),
                    Username = username,
                    ClientId = clientId,
                    IssuedUtc = DateTime.UtcNow,
                    ExpiresUtc = DateTime.UtcNow.AddSeconds(refreshTokenTTL)
                };

                // Set token
                context.Ticket.Properties.IssuedUtc = rt.IssuedUtc;
                context.Ticket.Properties.ExpiresUtc = rt.ExpiresUtc;

                // Serialize the ticket
                rt.Ticket = context.SerializeTicket();

                // Add to refresh tokens table
                ctx.RefreshTokens.Add(rt);

                // Save
                await ctx.SaveChangesAsync();
            }

            // Add original token to ticket
            context.SetToken(token);

        }

        public async Task ReceiveAsync(AuthenticationTokenReceiveContext context)
        {

            using (var ctx = new ApplicationDbContext())
            {
                // Get the hash of our token
                var hash = Encryption.Hash(context.Token);

                // Find the refresh token by its hash
                var rt = (from r in ctx.RefreshTokens
                          where r.Id == hash
                          select r).SingleOrDefault();

                if (rt != null)
                {
                    // Get ticket from stored data
                    context.DeserializeTicket(rt.Ticket);
                    // Delete the token from the DB
                    ctx.RefreshTokens.Remove(rt);

                    // Save changes
                    await ctx.SaveChangesAsync();

                }
                
            }

        }

        public void Create(AuthenticationTokenCreateContext context)
        {
            throw new NotImplementedException();
        }

        public void Receive(AuthenticationTokenReceiveContext context)
        {
            throw new NotImplementedException();
        }
    }


}