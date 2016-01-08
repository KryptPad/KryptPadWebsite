using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KryptPadWebApp.Controllers
{
    [RoutePrefix("Account")]
    public class AccountController : SecureController
    {
        // GET: Account
        [Route("Login", Name = "Login")]
        public ActionResult Login()
        {
            return View();
        }

        // GET: Profiles
        [Route("Profiles", Name = "Profiles")]
        public ActionResult Profiles()
        {
            return View();
        }
    }
}