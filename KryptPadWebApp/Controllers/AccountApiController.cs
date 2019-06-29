using KryptPadWebApp.Cryptography;
using KryptPadWebApp.Email;
using KryptPadWebApp.Helpers;
using KryptPadWebApp.Models;
using KryptPadWebApp.Models.Requests;
using KryptPadWebApp.Models.Results;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Linq;

namespace KryptPadWebApp.Controllers
{
    [RoutePrefix("api/account")]
    public class AccountApiController : ApiController
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

        #region Account management

        /// <summary>
        /// Registers an account
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("register")]
        public async Task<IHttpActionResult> Register(CreateAccountRequest model)
        {
            // Check if we have a valid model state
            if (!ModelState.IsValid)
            {
                // Return the error
                return BadRequest(ModelState);
            }

            // Create the user in the database
            var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
            var result = await UserManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                // We can't authorize the device if we have no app ID. It is probably an old app
                if (model.AppId != Guid.Empty)
                {
                    // The device must be authorized before the user can sign in. Since
                    // we are signing up, automatically authorize the device
                    await AuthorizedDeviceHelper.AddAuthorizedDevice(user.Id, model.AppId, HttpContext.Current.Request.UserHostAddress);
                }

                // Send confirm link
                await SendConfirmEmail(user.Id);

                // All is ok
                return Ok();
            }
            else
            {
                // Add errors to the model state
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("Errors", error);
                }

                // Return the error
                return BadRequest(ModelState);
            }
        }

        /// <summary>
        /// Gets some details about the account
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("details")]
        [Authorize]
        public async Task<IHttpActionResult> AccountDetails()
        {
            // Find the user by his/her id
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                // Get some info
                var details = new AccountDetailsResult()
                {
                    EmailConfirmed = user.EmailConfirmed,
                    EmailHash = Encryption.ByteArrayToHex(Encryption.HashMD5(User.Identity.GetUserName()))
                };

                return Json(details);
            }
            else
            {
                // User not found?
                return BadRequest();
            }
        }

        /// <summary>
        /// Sends a confirmation email to the user
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("send-email-confirmation-link")]
        [Authorize]
        public async Task<IHttpActionResult> SendEmailConfirmationLink(ApplicationUser user = null)
        {
            if (user == null)
            {
                // Find user by id
                user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            }

            // Check if the user is found and not already confirmed
            if (user != null && !user.EmailConfirmed)
            {
                // Send confirm link
                await SendConfirmEmail(user.Id);

                // All is ok
                return Ok();
            }
            else
            {
                return BadRequest("Email address is already confirmed.");
            }


        }

        [HttpPost]
        [Route("deauthorize-devices")]
        [Authorize]
        public async Task<IHttpActionResult> DeauthorizeAllDevices()
        {
            // Find user by id
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                using (var ctx = new ApplicationDbContext())
                {

                    // Sign-out of all devices
                    var tokens = (from t in ctx.RefreshTokens
                                  where t.Username == user.Email
                                  select t).ToArray();

                    // Delete all refresh tokens
                    ctx.RefreshTokens.RemoveRange(tokens);

                    // Get all devices for the user
                    var devices = (from d in ctx.AuthorizedDevices
                                   where d.User.Id == user.Id
                                   select d).ToArray();

                    // Delete them all
                    ctx.AuthorizedDevices.RemoveRange(devices);
                    // Commit
                    await ctx.SaveChangesAsync();

                }

                // Invalidate tokens
                await UserManager.UpdateSecurityStampAsync(user.Id);

                // All is ok
                return Ok();

            }
            else
            {
                return BadRequest("User does not exist.");
            }

        }

        /// <summary>
        /// Deletes the user's account
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [Route("")]
        [Authorize]
        public async Task<IHttpActionResult> DeleteAccount()
        {
            // Find user by id
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                // Get the user's email address
                var email = user.Email;
                try
                {
                    // Delete the account
                    await UserManager.DeleteAsync(user);

                    // Send the email
                    await EmailHelper.SendAsync("Your account has been deleted.", "You're account has been successfully deleted. If you did not initiate this, contact support immediately.", email);

                }
                catch (Exception ex)
                {

                    throw ex;
                }

                // All is ok
                return Ok();

            }
            else
            {
                return BadRequest("User does not exist.");
            }


        }

        #endregion

        #region Password management

        /// <summary>
        /// Sends the user an email with a link to reset their password
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("forgot-password")]
        public async Task<IHttpActionResult> ForgotPassword(ForgotPasswordRequest model)
        {
            if (ModelState.IsValid)
            {
                // Get the user from the email address
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user != null && !user.EmailConfirmed)
                {
                    // User is not verified, resend verification email
                    await SendEmailConfirmationLink(user);
                    return Ok();
                }
                if (user == null)
                {
                    // Don't send anything, the user does not exist
                    return Ok();
                }

                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                var callbackUrl = $"{Request.RequestUri.Scheme}://{Request.RequestUri.Authority}{Url.Route("ChangePassword", new { userId = user.Id, code })}";

                // Send the email
                await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");

                return Ok();

            }

            // Oops
            return BadRequest();
        }

        //[HttpPost]
        //[Route("Reset-Password")]
        //public async Task<IHttpActionResult> ResetPassword(ResetPasswordRequest model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    // Reset the user's password
        //    var result = await UserManager.ResetPasswordAsync(model.UserId, model.Code, model.Password);
        //    if (result.Succeeded)
        //    {
        //        return Ok();
        //    }
        //    else
        //    {
        //        // Add errors
        //        foreach (var err in result.Errors)
        //        {
        //            ModelState.AddModelError("", err);
        //        }
        //        // Return errors
        //        return BadRequest(ModelState);
        //    }


        //}

        ///// <summary>
        ///// Changes the user's account password
        ///// </summary>
        ///// <param name="model"></param>
        ///// <returns></returns>
        //[HttpPost]
        //[Route("Change-Password")]
        //[Authorize]
        //public async Task<IHttpActionResult> ChangePassword(ChangePasswordRequest model)
        //{
        //    if (model == null || !ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    // Change the password
        //    var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.CurrentPassword, model.NewPassword);
        //    if (result.Succeeded)
        //    {
        //        return Ok();
        //    }

        //    return BadRequest("Your account password could not be changed. Please make sure you entered the correct current password.");
        //}


        #endregion


        /// <summary>
        /// This method does nothing except return OK (200)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("Test")]
        public IHttpActionResult Test()
        {
            return Ok();
        }

        #region Helper methods

        private async Task SendConfirmEmail(string userId)
        {
            // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
            // Send an email with this link
            string code = await UserManager.GenerateEmailConfirmationTokenAsync(userId);
            var callbackUrl = $"{Request.RequestUri.Scheme}://{Request.RequestUri.Authority}{Url.Route("ConfirmEmail", new { userId, code })}";

            // Send the email
            await UserManager.SendEmailAsync(userId, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

        }
        #endregion
    }

}