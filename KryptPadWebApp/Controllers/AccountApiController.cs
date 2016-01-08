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



namespace KryptPadWebApp.Controllers
{
    [RoutePrefix("Api/Account")]
    public class AccountApiController : ApiController
    {

        //POST: Register
        [HttpPost]
        [Route("Register", Name = "Register")]
        public async Task<HttpResponseMessage> Register(CreateAccount accountInfo)
        {
            var userManager = Request.GetOwinContext().GetUserManager<ApplicationUserManager>();

            //create the user in the database
            var user = new ApplicationUser { UserName = accountInfo.Email, Email = accountInfo.Email };
            var result = await userManager.CreateAsync(user, accountInfo.Password);
            if (result.Succeeded)
            {

                //all is ok
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            else
            {
                //add errors to the model state
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("Errors", error);
                }

                //return the error
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }
    }

    public class CreateAccount
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}