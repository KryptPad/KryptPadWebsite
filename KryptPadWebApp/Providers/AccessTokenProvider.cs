using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using KryptPadWebApp.Models;

namespace KryptPadWebApp.Providers
{

    public class AccessTokenProvider : OAuthAuthorizationServerProvider
    {
        private readonly string _publicClientId;

        public AccessTokenProvider(string publicClientId)
        {
            if (publicClientId == null)
            {
                throw new ArgumentNullException("publicClientId");
            }

            _publicClientId = publicClientId;
        }

        /// <summary>
        /// Validates the client id
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {

            string clientId;
            string clientSecret;
            // Gets the clientid and client secret from authenticate header
            if (!context.TryGetBasicCredentials(out clientId, out clientSecret))
            {
                // try to get form values
                context.TryGetFormCredentials(out clientId, out clientSecret);

            }

            // Validate clientid and clientsecret. You can omit validating client secret if none is provided in your request (as in sample client request above)
            var validClient = !string.IsNullOrWhiteSpace(clientId);

            if (validClient)
            {
                // Need to make the client_id available for later security checks
                context.OwinContext.Set<string>("as:client_id", clientId);

                context.Validated();
            }
            else
            {
                context.Rejected();
            }

            return Task.FromResult(0);

        }

        /// <summary>
        /// Validates a request for an access token based on username and password
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            // Get the user manager
            var userManager = context.OwinContext.GetUserManager<ApplicationUserManager>();

            // Find the user based on the credentials provided
            var user = await userManager.FindAsync(context.UserName, context.Password);

            if (user == null)
            {
                context.SetError("invalid_grant", "The username or password is incorrect.");
                context.Rejected();
                return;
            }

            var oAuthIdentity = await user.GenerateUserIdentityAsync(userManager, OAuthDefaults.AuthenticationType);
            //ClaimsIdentity cookiesIdentity = await user.GenerateUserIdentityAsync(userManager, CookieAuthenticationDefaults.AuthenticationType);

            // Create ticket
            var ticket = new AuthenticationTicket(
                oAuthIdentity,
                CreateProperties(user.UserName, context.ClientId));

            // Validate our request for a token
            context.Validated(ticket);
            //context.Request.Context.Authentication.SignIn(cookiesIdentity);
        }

        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }

            return Task.FromResult<object>(null);
        }

        public override Task ValidateClientRedirectUri(OAuthValidateClientRedirectUriContext context)
        {
            if (context.ClientId == _publicClientId)
            {
                Uri expectedRootUri = new Uri(context.Request.Uri, "/");

                if (expectedRootUri.AbsoluteUri == context.RedirectUri)
                {
                    context.Validated();
                }
            }

            return Task.FromResult<object>(null);
        }

        public static AuthenticationProperties CreateProperties(string userName, string clientId)
        {
            IDictionary<string, string> data = new Dictionary<string, string>
            {
                { "as:client_id", (clientId == null) ? string.Empty : clientId },
                { "userName", userName }
            };
            return new AuthenticationProperties(data);
        }

        public override Task GrantRefreshToken(OAuthGrantRefreshTokenContext context)
        {
            var originalClient = context.Ticket.Properties.Dictionary["as:client_id"];
            var currentClient = context.OwinContext.Get<string>("as:client_id");

            // enforce client binding of refresh token
            if (originalClient != currentClient)
            {
                context.Rejected();
            }

            // chance to change authentication ticket for refresh token requests
            var newId = new ClaimsIdentity(context.Ticket.Identity);
            newId.AddClaim(new Claim("newClaim", "refreshToken"));

            var newTicket = new AuthenticationTicket(newId, context.Ticket.Properties);
            context.Validated(newTicket);

            return Task.FromResult(0);
        }
    }

}