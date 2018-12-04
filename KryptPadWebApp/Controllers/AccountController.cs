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
        public async Task<ActionResult> AuthorizeDevice(string userId, string code, string appId)
        {
            // Confirm the email address
            var success = await UserManager.VerifyUserTokenAsync(userId, "AuthorizeDevice-" + appId, code);
            if (success)
            {
                var authorized = await AuthorizedDeviceHelper.AddAuthorizedDevice(userId, Guid.Parse(appId));
                if (!authorized)
                {
                    ModelState.AddModelError("", "Failed to authorize your device.");
                }
                
            }

            return View();
        }
    }
}