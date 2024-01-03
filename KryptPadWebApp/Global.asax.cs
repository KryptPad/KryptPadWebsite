using KryptPadWebApp.Migrations;
using KryptPadWebApp.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

using System.Web.Configuration;
using System.Web.Security;
using System.Web.Hosting;
using KryptPadWebApp.Email;


namespace KryptPadWebApp
{
    public class MvcApplication : System.Web.HttpApplication
    {
        //Non-www requests will be permanently redirected to www.
        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            if (!Request.Url.Host.StartsWith("www") && !Request.Url.IsLoopback)
            {
                UriBuilder builder = new UriBuilder(Request.Url)
                {
                    Host = "www." + Request.Url.Host
                };
                Response.RedirectPermanent(builder.ToString(), true);
            }
        }

        protected void Application_Start()
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<ApplicationDbContext, Configuration>());
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
             
    }
}
