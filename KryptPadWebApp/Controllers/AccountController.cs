using KryptPadWebApp.Helpers;
using KryptPadWebApp.Models.Requests;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace KryptPadWebApp.Controllers
{
    [RoutePrefix("account")]
    [Authorize]
    public class AccountController : Controller
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

        // GET: Account
        [HttpGet]
        [Route("authorize-device")]
        [AllowAnonymous]
        public async Task<ActionResult> AuthorizeDevice(string userId, string code, string appId, string ipAddress)
        {
            try
            {
                // Confirm the email address
                var success = await UserManager.VerifyUserTokenAsync(userId, "AuthorizeDevice-" + appId, code);
                if (success)
                {
                    var authorized = await AuthorizedDeviceHelper.AddAuthorizedDevice(userId, Guid.Parse(appId), ipAddress);
                    if (authorized)
                    {
                        return View();
                    }

                }

                throw new Exception("Failed to authorize this device for this user.");

            }
            catch (Exception)
            {
                return View("Error");
            }


        }

        /// <summary>
        /// This route comes from the link in the confirm email message sent out to the user.
        /// When they click on it, it takes them here. We return to the result page or throw
        /// an error if not successful.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("confirm-email", Name = "ConfirmEmail")]
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {

            try
            {
                // Confirm the email address
                var result = await UserManager.ConfirmEmailAsync(userId, code);
                if (result.Succeeded)
                {
                    return View();
                }
                else
                {
                    throw new Exception("User does not exist");
                }

            }
            catch (Exception)
            {
                return View("Error");
            }


        }

        /// <summary>
        /// This route comes from the reset password email.
        /// When the user clicks the link, it takes them here so they can update their password.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        // GET: ResetPassword
        [Route("change-password", Name = "ChangePassword")]
        [AllowAnonymous]
        public async Task<ActionResult> ChangePassword(string userId, string code)
        {
            var result = await UserManager.VerifyUserTokenAsync(userId, "ResetPassword", code);
            if (!result)
            {
                return View("Error");
            }
            else
            {
                var m = new ResetPasswordModel()
                {
                    UserId = userId,
                    Code = code
                };

                return View(m);
            }

        }

        /// <summary>
        /// User posts new password
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("change-password")]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<ActionResult> ChangePassword(ResetPasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Reset the user's password
            var result = await UserManager.ResetPasswordAsync(model.UserId, model.Code, model.Password);
            if (result.Succeeded)
            {
                model.Success = true;
                return View(model);
            }
            else
            {
                // Add errors
                foreach (var err in result.Errors)
                {
                    ModelState.AddModelError("", err);
                }
                // Return errors
                return View(model);
            }
        }
    }
}