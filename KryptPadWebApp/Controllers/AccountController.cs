using KryptPadWebApp.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace KryptPadWebApp.Controllers
{

    /// <summary>
    /// Provides account security methods.
    /// </summary>
    [Authorize]
    [RoutePrefix("security")]
    public class AccountController : Controller
    {

        private ApplicationSignInManager _signInManager;

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        private ApplicationUserManager _userManager;

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        // POST: Signin
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Signin", Name = "DoSigninRoute")]
        public async Task<ActionResult> SignIn(SigninModel model)
        {

            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, true, shouldLockout: false);
            if (result == SignInStatus.Success)
            {
                return RedirectToRoute("SelectProfileRoute");
            }


            return RedirectToRoute("Home");
        }
        
    }
}