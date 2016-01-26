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
    }

}