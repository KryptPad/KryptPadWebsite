﻿using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace KryptPadWebApp.Controllers
{
    [RoutePrefix("Api")]
    public class ApiInfoController : ApiController
    {
        [Route("Get-Routes", Name = "GetRoutes")]
        public async Task<IHttpActionResult> GetRoutes()
        {
            // Get routes
            var routes = new
            {
                Signin = Url.Route("DoSignIn", null)
            };

            // Serialization settings
            var ss = new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            var json = JsonConvert.SerializeObject(routes, ss);
            // Create a javascript object
            var js = $"var routes = {json}";

            return await Task.Factory.StartNew(() => JavaScript(js));
        }

        /// <summary>
        /// Returns a string as a JavaScript file
        /// </summary>
        /// <param name="js"></param>
        /// <returns></returns>
        private IHttpActionResult JavaScript(string js, bool noCache = false)
        {
            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(js, Encoding.UTF8, "application/javascript");
            if (!noCache)
            {
                response.Headers.CacheControl = new CacheControlHeaderValue()
                {
                    Public = true,
                    MaxAge = TimeSpan.FromDays(1)
                };
            }
            return ResponseMessage(response);
        }
    }
}
