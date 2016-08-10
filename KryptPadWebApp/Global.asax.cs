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
        protected void Application_Start()
        {

            // Protect the web.config
            EncryptConfigSection("connectionStrings");
            EncryptConfigSection("appSettings");


            Database.SetInitializer(new MigrateDatabaseToLatestVersion<ApplicationDbContext, Configuration>());
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        /// <summary>
        /// Encrypts the web.config connection string section
        /// </summary>
        public void EncryptConfigSection(string sectionKey)
        {
            try
            {
                // Get config
                var config = WebConfigurationManager.OpenWebConfiguration("~");
                // Open connection string section
                var section = config.GetSection(sectionKey);

                // If the section is not protected, protect it
                if (section != null && !section.SectionInformation.IsProtected && !section.ElementInformation.IsLocked)
                {
                    // Protect section
                    section.SectionInformation.ProtectSection("DataProtectionConfigurationProvider");
                    section.SectionInformation.ForceSave = true;
                    // Save config
                    config.Save(System.Configuration.ConfigurationSaveMode.Full);
                }
            }
            catch (Exception ex)
            {
                // Log the error
                EmailHelper.SendAsync(
                    nameof(EncryptConfigSection),
                    ex.Message,
                    System.Configuration.ConfigurationManager.AppSettings["ErrorEmailDestination"]);
            }

        }

    }
}
