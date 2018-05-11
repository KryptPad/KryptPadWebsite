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
        #region Properties
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
        #endregion

        [AllowAnonymous]
        [Route("", Name = "Home")]
        [Route("SignIn", Name = "SignIn")]
        public ActionResult Index()
        {
            // Create the view model for our view
            var model = new HomeIndexViewModel();
            
            return View(model);
        }

        #region Account signin management

        /// <summary>
        /// Signs the user into the system
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("SignIn", Name = "DoSignIn")]
        public async Task<ActionResult> SignIn(SigninModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", new HomeIndexViewModel());
            }


            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    // Select profile
                    return RedirectToRoute("SelectProfile");
                //case SignInStatus.RequiresVerification:
                //    break;
                //case SignInStatus.LockedOut:
                //    break;
                default:
                    // Alert user the login failed
                    ModelState.AddModelError("", "Username or password is incorrect.");
                    return View("Index", new HomeIndexViewModel());

            }

        }

        /// <summary>
        /// Signs the user out of the system
        /// </summary>
        /// <returns></returns>
        [Route("SignOut", Name = "DoSignOut")]
        public async Task<ActionResult> SignOut()
        {
            SignInManager.AuthenticationManager.SignOut();

            return await Task.Factory.StartNew(() => { return RedirectToRoute("Home"); });
        }

        #endregion

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