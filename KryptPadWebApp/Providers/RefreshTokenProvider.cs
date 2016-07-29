using KryptPadWebApp.Cryptography;
using KryptPadWebApp.Models;
using KryptPadWebApp.Models.Entities;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Infrastructure;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace KryptPadWebApp.Providers
{
    public class RefreshTokenProvider : IAuthenticationTokenProvider
    {

        public async Task CreateAsync(AuthenticationTokenCreateContext context)
        {
            // Generate token guid
            var guid = Guid.NewGuid().ToString();
            var clientId = context.Ticket.Properties.Dictionary["as:client_id"];

            // Create some properties for our ticket
            var refreshTokenProperties = new AuthenticationProperties(context.Ticket.Properties.Dictionary)
            {
                IssuedUtc = context.Ticket.Properties.IssuedUtc,
                ExpiresUtc = DateTime.UtcNow.AddYears(1)
            };

            // Create token ticket
            var refreshTokenTicket = new AuthenticationTicket(context.Ticket.Identity, refreshTokenProperties);
            
            // Store the refresh token in the database
            using (var ctx = new ApplicationDbContext())
            {
                // Create refresh token
                var rt = new RefreshToken()
                {
                    Id = Encryption.Hash(guid),
                    Username = context.Ticket.Identity.Name,
                    ClientId = clientId,
                    IssuedUtc = DateTime.UtcNow,
                    ExpiresUtc = DateTime.UtcNow.AddDays(14)
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

            //_refreshTokens.TryAdd(guid, context.Ticket);
            //_refreshTokens.TryAdd(Encryption.Hash(guid), refreshTokenTicket);

            // Add original token to ticket
            context.SetToken(guid);

            //return Task.FromResult(0);
        }

        public async Task ReceiveAsync(AuthenticationTokenReceiveContext context)
        {
            Guid token;

            if (Guid.TryParse(context.Token, out token))
            {
                using(var ctx = new ApplicationDbContext())
                {
                    // Get the hash of our token
                    var hash = Encryption.Hash(token.ToString());

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