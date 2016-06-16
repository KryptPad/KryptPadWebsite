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

        // GET: SignIn
        [Route("SignIn", Name = "SignIn")]
        public ActionResult SignIn()
        {
            return View();
        }

        // GET: SignUp
        [Route("SignUp", Name = "SignUp")]
        public ActionResult SignUp()
        {
            return View();
        }

        // GET: ForgotPassword
        [Route("ForgotPassword", Name = "ForgotPassword")]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        // GET: ResetPassword
        [Route("ResetPassword", Name = "ResetPassword")]
        public ActionResult ResetPassword()
        {
            return View();
        }
    }
}