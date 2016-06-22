using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace KryptPadWebApp.Controllers
{
    [RoutePrefix("App")]
    public class AppController : Controller
    {
        /// <summary>
        /// Gets the user manager from the owin context
        /// </summary>
        private ApplicationUserManager UserManager
        {
            get
            {
                return Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
        }

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
        public ActionResult ResetPassword(string userId, string code)
        {
            return View();
        }

        // GET: ResetPassword
        [Route("ConfirmEmail", Name = "ConfirmEmail")]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (!ModelState.IsValid)
            {
                return View("Error");
            }

            // Confirm the email address
            var result = await UserManager.ConfirmEmailAsync(userId, code);

            // Return OK if the account was confirmed successfully
            if (result.Succeeded)
            {
                return View();
            }
            else
            {
                return View("Error");
            }
            
        }
    }
}