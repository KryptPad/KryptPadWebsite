using KryptPadWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace KryptPadWebApp.Controllers
{
    [RoutePrefix("")]
    public class HomeController : SecureController
    {
        [Route("", Name = "Home")]
        public ActionResult Index()
        {
            // Create the view model for our view
            var model = new HomeIndexViewModel();

            return View(model);
        }

        [Route("Download", Name = "Download")]
        public async Task<FileResult> Download()
        {
            // Map path to the download file
            var pathName = Server.MapPath("~/App_Data/krypt-pad-dekrypter.zip");

            // Return the file
            return File(pathName, "application/octet-stream", "krypt-pad-dekrypter.zip");
        }

        [Route("Policy", Name = "Policy")]
        public ActionResult Policy()
        {
            return View();
        }

        [Route("Terms", Name = "Terms")]
        public ActionResult Terms()
        {
            return View();
        }

        [Route("About", Name = "About")]
        public ActionResult About()
        {
            return View();
        }

        [Route("Dekrypter", Name = "Dekrypter")]
        public ActionResult Dekrypter()
        {
            return View();
        }
    }
}