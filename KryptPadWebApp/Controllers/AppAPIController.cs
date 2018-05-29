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
        public HttpResponseMessage GetBroadcastMessage()
        {
            using (var ctx = new ApplicationDbContext())
            {
                var appSettings = ctx.AppSettings.FirstOrDefault();
                if (appSettings != null)
                {
                    // Get the broadcast message and return it
                    if (!string.IsNullOrWhiteSpace(appSettings.BroadcastMessage))
                    {
                        var resp = new HttpResponseMessage(HttpStatusCode.OK);
                        resp.Content = new StringContent(appSettings.BroadcastMessage, System.Text.Encoding.UTF8, "text/plain");
                        return resp;

                    }

                }
                
            }

            // Nothing to show here
            return new HttpResponseMessage(HttpStatusCode.NoContent);
        }

    }
}
