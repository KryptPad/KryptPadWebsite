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
using KryptPadWebApp.Email;
using System.Web;

namespace KryptPadWebApp.Providers
{

    public class AccessTokenProvider : OAuthAuthorizationServerProvider
    {
        private readonly string _publicClientId;

        public AccessTokenProvider(string publicClientId)
        {
            _publicClientId = publicClientId ?? throw new ArgumentNullException("publicClientId");
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
            var validClient = true;//!string.IsNullOrWhiteSpace(clientId);

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
            // DISABLED!
            context.Rejected();
            context.SetError("invalid_grant", "Logins are temporarily disabled due to web host outtage.");
            return;

            // Get the user manager
            var userManager = context.OwinContext.GetUserManager<ApplicationUserManager>();
            // Read the form data
            var formData = await context.Request.ReadFormAsync();
            // Get the app id
            var appId = formData["app_id"] != null ? Guid.Parse(formData["app_id"]) : Guid.Empty;

            // Find the user based on the credentials provided
            var user = await userManager.FindAsync(context.UserName, context.Password);
            if (user == null)
            {
                context.Rejected();
                context.SetError("invalid_grant", "The username or password is incorrect.");
                
                return;
            }

            // Now that we have a user, look up the authorized devices. This will prevent someone from logging in with
            // the username and password until the device is authorized. An email will be sent to the user with a link
            // to add the device if it is the first time they are using it. Once they click the link, the device
            // will be added, and the user can then log in.
            if (appId != Guid.Empty)
            {
                using (var ctx = new ApplicationDbContext())
                {
                    var ipAddress = context.Request.RemoteIpAddress;
                    // Look up id in authorized devices. If it is not there, send an email to the user
                    var authorizedId = ctx.AuthorizedDevices.Where(x => x.User.Id == user.Id && x.AppId == appId && x.AccessedFromIPAddress == ipAddress).FirstOrDefault();
                    if (authorizedId == null)
                    {
                        var code = await userManager.GenerateUserTokenAsync("AuthorizeDevice-" + appId, user.Id);
                        var link = $"<a href=\"{context.Request.Scheme}://{context.Request.Host}/account/authorize-device?userId={user.Id}&code={HttpUtility.UrlEncode(code)}&appId={appId}&ipAddress={context.Request.RemoteIpAddress}\">Authorize device</a>";

                        // Send email and set error message
                        await EmailHelper.SendAsync("Authorize Device", $"Your username and password was used to log in to Krypt Pad, but the device was not recognized.<br /><br />Date (UTC): {DateTime.UtcNow}<br />IP Address: {context.Request.RemoteIpAddress}<br /><br />If this was you, click the link to authorize this device. {link}", user.Email);

                        // Reject the login
                        context.Rejected();
                        context.SetError("invalid_grant", "Before you can log in, you must first authorize this device. We sent you an email with a link. Click the link to authorize this device and try logging in again.");
                        return;
                    }
                }
            }

            var oAuthIdentity = await user.GenerateUserIdentityAsync(userManager, OAuthDefaults.AuthenticationType);

            // Create ticket
            var ticket = new AuthenticationTicket(
                oAuthIdentity,
                CreateProperties(user.UserName, context.ClientId));

            // Set last logged in time
            user.LastLoggedIn = DateTime.UtcNow;
            // Update the user
            await userManager.UpdateAsync(user);

            // Validate our request for a token
            context.Validated(ticket);
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

            // Enforce client binding of refresh token
            if (originalClient != currentClient)
            {
                context.Rejected();
                context.SetError("invalid_grant", "Access denied.");

                return Task.FromResult(0);
            }

            // Chance to change authentication ticket for refresh token requests
            var newId = new ClaimsIdentity(context.Ticket.Identity);
            newId.AddClaim(new Claim("newClaim", "refreshToken"));

            var newTicket = new AuthenticationTicket(newId, context.Ticket.Properties);
            context.Validated(newTicket);

            return Task.FromResult(0);
        }
    }

}