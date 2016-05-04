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

        //POST: Register
        [HttpPost]
        [Route("Register", Name = "Register")]
        public async Task<IHttpActionResult> Register(ApiAccount accountInfo)
        {
            // Check if we have a valid model state
            if (!ModelState.IsValid)
            {
                // Return the error
                return BadRequest(ModelState);
            }

            // If we have a model state, then get the user manager
            var userManager = Request.GetOwinContext().GetUserManager<ApplicationUserManager>();

            // Create the user in the database
            var user = new ApplicationUser { UserName = accountInfo.Email, Email = accountInfo.Email };
            var result = await userManager.CreateAsync(user, accountInfo.Password);
            if (result.Succeeded)
            {

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
        [Route("ForgotPassword", Name = "ForgotPassword")]
        public async Task<IHttpActionResult> ForgotPassword(ForgotPasswordRequest request)
        {
            if (ModelState.IsValid)
            {

                // If we have a model state, then get the user manager
                var userManager = Request.GetOwinContext().GetUserManager<ApplicationUserManager>();

                var user = await userManager.FindByNameAsync(request.Email);
                if (user == null || !(await userManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return Ok();
                }

                try
                {
                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    string code = await userManager.GeneratePasswordResetTokenAsync(user.Id);
                    var callbackUrl = $"{Request.RequestUri.Scheme}://{Request.RequestUri.Host}/api/account/reset-password?userId={user.Id}&code={code}";


                    await userManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");

                    return Ok();
                }
                catch (Exception ex)
                {
                    throw;
                }
                
            }

            // Oops
            return BadRequest();
        }

        [HttpGet]
        [Route("ResetPassword", Name = "ResetPassword")]
        public async Task<IHttpActionResult> ResetPassword(string userId, string code)
        {
            return Ok();
        }

    }

}