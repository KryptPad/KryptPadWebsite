using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.AspNet.Identity;

namespace KryptPadWebApp.Controllers
{
    [Authorize]
    public class AuthorizedApiController : ApiController
    {
        /// <summary>
        /// Gets the authenticated user's id
        /// </summary>
        protected string UserId
        {
            get
            {
                return User.Identity.GetUserId();
            }
        }

        /// <summary>
        /// Gets the passphrase sent in the header
        /// </summary>
        protected string Passphrase
        {
            get
            {
                // Get the passphrase from the header
                var passphrase = Request.Headers.GetValues("Passphrase").FirstOrDefault();
                return passphrase;
            }
        }

    }
}