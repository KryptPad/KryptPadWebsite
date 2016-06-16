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

        //POST: Register
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

                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                var callbackUrl = $"{Request.RequestUri.Scheme}://{Request.RequestUri.Authority}/app#confirm-email?userId={user.Id}&code={HttpUtility.UrlEncode(code)}";

                // Send the email
                await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");


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

        [HttpPost]
        [Route("Forgot-Password")]
        public async Task<IHttpActionResult> ForgotPassword(ForgotPasswordRequest model)
        {
            if (ModelState.IsValid)
            {

                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return Ok();
                }

                
                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                var callbackUrl = $"{Request.RequestUri.Scheme}://{Request.RequestUri.Authority}/app#reset-password?userId={user.Id}&code={HttpUtility.UrlEncode(code)}";

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

            // Get the user based on the email address
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user != null)
            {
                var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
                if (result.Succeeded)
                {
                    return Ok();
                }
            }

            return Ok();
        }

        /// <summary>
        /// Confirm account
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Confirm-Email", Name = "ConfirmEmail")]
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

    }

}