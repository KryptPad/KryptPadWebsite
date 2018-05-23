using KryptPadWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace KryptPadWebApp.Controllers
{
    [RoutePrefix("api/app")]
    public class AppAPIController : ApiController
    {

        /// <summary>
        /// Gets the system broadcast message
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("broadcast-message")]
        public IHttpActionResult GetBroadcastMessage()
        {
            using (var ctx = new ApplicationDbContext())
            {
                var appSettings = ctx.AppSettings.FirstOrDefault();
                if (appSettings != null)
                {
                    // Get the broadcast message and return it
                    if (!string.IsNullOrWhiteSpace(appSettings.BroadcastMessage))
                    {
                        return Ok(appSettings.BroadcastMessage);
                    }
                   
                }
                
            }

            // Nothing to show here
            return StatusCode(HttpStatusCode.NoContent);
        }

    }
}
