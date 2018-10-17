using Elmah.Contrib.WebApi;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;

namespace KryptPadWebApp
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Enable elmah
            config.Services.Add(typeof(IExceptionLogger), new ElmahExceptionLogger());
            // Web API configuration and services
            config.SuppressDefaultHostAuthentication();
            config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));
            // Web API routes
            config.MapHttpAttributeRoutes();

        }
    }
}
