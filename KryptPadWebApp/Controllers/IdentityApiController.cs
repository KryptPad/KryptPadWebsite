using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace KryptPadWebApp.Controllers
{
    [RoutePrefix(".well-known/openid-configuration")]
    public class IdentityApiController : ApiController
    {
        // GET api/<controller>
        [Route("")]
        [HttpGet]
        public IHttpActionResult Get()
        {
            // This is a shim to prepare for a server migratoin and to use IdentityServer4. For now, we
            // will be providing the values that this site uses, but the new site will have new values
            // which the app will read and use.
            var resp = new { token_endpoint = $"{Request.RequestUri.Scheme }://{Request.RequestUri.Authority}/token" };

            return Json(resp);
        }


    }
}