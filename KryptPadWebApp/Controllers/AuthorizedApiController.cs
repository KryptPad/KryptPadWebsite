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
        protected string UserId
        {
            get
            {
                return User.Identity.GetUserId();
            }
        }
    }
}