using KryptPadWebApp.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Owin.Security;
using Microsoft.AspNet.Identity.Owin;
using System.Web.Http;
using KryptPadWebApp.Models.ApiEntities;
using KryptPadWebApp.Models.Requests;
using KryptPadWebApp.Models.Results;
using KryptPadWebApp.Cryptography;

namespace KryptPadWebApp.Controllers
{
    [RoutePrefix("Api/Account")]
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

        /// <summary>
        /// Registers an account
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Register", Name = "Register")]
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
                // Send confirm link
                await SendEmailConfirmationLink(user.Id);

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
        /// Sends the user an email with a link to reset their password
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Forgot-Password")]
        public async Task<IHttpActionResult> ForgotPassword(ForgotPasswordRequest model)
        {
            if (ModelState.IsValid)
            {
                // Get the user from the email address
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return Ok();
                }

                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link HttpUtility.UrlEncode(code)
                string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                var callbackUrl = $"{Request.RequestUri.Scheme}://{Request.RequestUri.Authority}{Url.Route("ResetPassword", new { userId = user.Id, code = code })}";

                // Send the email
                await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");

                return Ok();

            }

            // Oops
            return BadRequest();
        }

        [HttpPost]
        [Route("Reset-Password")]
        public async Task<IHttpActionResult> ResetPassword(ResetPasswordRequest model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Reset the user's password
            var result = await UserManager.ResetPasswordAsync(model.UserId, model.Code, model.Password);
            if (result.Succeeded)
            {
                return Ok();
            }
            else
            {
                // Add errors
                foreach (var err in result.Errors)
                {
                    ModelState.AddModelError("", err);
                }
                // Return errors
                return BadRequest(ModelState);
            }


        }

        /// <summary>
        /// Changes the user's account password
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Change-Password")]
        [Authorize]
        public async Task<IHttpActionResult> ChangePassword(ChangePasswordRequest model)
        {
            if (model == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Change the password
            var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.CurrentPassword, model.NewPassword);
            if (result.Succeeded)
            {
                return Ok();
            }

            return BadRequest("Your account password could not be changed. Please make sure you entered the correct current password.");
        }

        /// <summary>
        /// Confirm account
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Confirm-Email")]
        public async Task<IHttpActionResult> ConfirmEmail(ConfirmEmailRequest model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Confirm the email address
            var result = await UserManager.ConfirmEmailAsync(model.UserId, model.Code);

            // Return OK if the account was confirmed successfully
            if (result.Succeeded)
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }


        }

        /// <summary>
        /// Gets some details about the account
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("Details")]
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
        [Route("Send-Email-Confirmation-Link")]
        [Authorize]
        public async Task<IHttpActionResult> SendEmailConfirmationLink()
        {
            // Find user by id
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null && !user.EmailConfirmed)
            {
                // Send confirm link
                await SendEmailConfirmationLink(user.Id);

                // All is ok
                return Ok();
            }
            else
            {
                return BadRequest("Email address is already confirmed.");
            }


        }

        ///// <summary>
        ///// This method does nothing except return OK (200) if authorized.
        ///// </summary>
        ///// <returns></returns>
        //[HttpGet]
        //[Authorize]
        //public IHttpActionResult Test()
        //{
        //    return Ok();
        //}

        #region Helper methods
        private async Task SendEmailConfirmationLink(string userId)
        {
            // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
            // Send an email with this link
            string code = await UserManager.GenerateEmailConfirmationTokenAsync(userId);
            var callbackUrl = $"{Request.RequestUri.Scheme}://{Request.RequestUri.Authority}{Url.Route("ConfirmEmail", new { userId = userId, code = code })}";

            // Send the email
            await UserManager.SendEmailAsync(userId, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

        }
        #endregion
    }

}