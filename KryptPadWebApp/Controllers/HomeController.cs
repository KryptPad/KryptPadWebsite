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
        public async Task<FileResult> DownloadFile()
        {
            // Map path to the download file
            var pathName = Server.MapPath("~/files/KryptPad.application");

            // Create the view model for our view
            var model = new HomeIndexViewModel();

            // Increase download count
            await model.IncrementDownloadCount();

            // Return the file
            return File(pathName, "application/octet-stream", "KryptPad.application");
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
    }
}