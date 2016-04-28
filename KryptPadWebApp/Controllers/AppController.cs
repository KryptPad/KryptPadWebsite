using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KryptPadWebApp.Controllers
{
    [RoutePrefix("App")]
    public class AppController : Controller
    {
        // GET: App
        [Route("", Name = "App")]
        public ActionResult Index()
        {
            return View();
        }
    }
}