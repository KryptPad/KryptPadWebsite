using KryptPadWebApp.Models;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace KryptPadWebApp.Controllers
{
    [Authorize]
    [RoutePrefix("")]
    public class HomeController : SecureController
    {
        [AllowAnonymous]
        [Route("", Name = "Home")]
        public ActionResult Index()
        {
            // Create the view model for our view
            var model = new HomeIndexViewModel();
            
            return View(model);
        }
        
        [AllowAnonymous]
        [Route("Policy", Name = "Policy")]
        public ActionResult Policy()
        {
            return View();
        }

        [AllowAnonymous]
        [Route("Terms", Name = "Terms")]
        public ActionResult Terms()
        {
            return View();
        }

        [AllowAnonymous]
        [Route("About", Name = "About")]
        public ActionResult About()
        {
            return View();
        }
        
    }
}