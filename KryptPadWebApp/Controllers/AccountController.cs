using KryptPadWebApp.Helpers;
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
    }
}