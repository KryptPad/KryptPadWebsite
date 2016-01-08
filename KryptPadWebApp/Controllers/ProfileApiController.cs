using KryptPadWebApp.Models;
using KryptPadWebApp.Models.Results;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;

namespace KryptPadWebApp.Controllers
{
    [Authorize]
    [RoutePrefix("Api/Profiles")]
    public class ProfileApiController : AuthorizedApiController
    {
        // GET api/<controller>
        [HttpGet]
        [Route("", Name = "ApiProfiles")]
        public IHttpActionResult Get()
        {
            //get the user's profiles
            using (var ctx = new ApplicationDbContext())
            {
                var profiles = (from p in ctx.Profiles
                                where p.User.Id == UserId
                                select p).ToArray();

                return Json(new ProfileResult(profiles));
            }

        }

        // GET api/<controller>/5
        [HttpGet]
        [Route("{id}")]
        public IHttpActionResult Get(int id)
        {
            //get the user's profiles
            using (var ctx = new ApplicationDbContext())
            {
                var profiles = (from p in ctx.Profiles
                                where p.User.Id == UserId
                                && p.Id == id
                                select p).ToArray();

                return Json(new ProfileResult(profiles));
            }
        }

        // POST api/<controller>
        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> Post(Profile profile)
        {
            // Create context
            using (var ctx = new ApplicationDbContext())
            {
                // Find the user
                var user = ctx.Users.Find(UserId);

                if (user == null)
                {
                    return Content(HttpStatusCode.BadRequest, "User not found");
                }

                profile.User = user;

                // Add the profile to the context
                ctx.Profiles.Add(profile);
                // Save changes
                await ctx.SaveChangesAsync();
                // Get the id of the newly created record
                var id = profile.Id;

                // Return profile back to the caller. it will have the new id
                return Json(new ProfileResult(new[] { profile }));
            }

        }

        // PUT api/<controller>/5
        [HttpPut]
        [Route("")]
        public void Put(int id, Profile profile)
        {
        }

        // DELETE api/<controller>/5
        [HttpDelete]
        [Route("{id}")]
        public async Task<IHttpActionResult> Delete(int id)
        {
            // Create context
            using (var ctx = new ApplicationDbContext())
            {
                // Delete the profile
                var profile = (from p in ctx.Profiles
                               where p.User.Id == UserId
                               && p.Id == id
                               select p).SingleOrDefault();

                if (profile != null)
                {
                    // Remove from the context
                    ctx.Profiles.Remove(profile);

                    // Save changes
                    await ctx.SaveChangesAsync();

                    return Ok();
                }
                else
                {
                    return Content(HttpStatusCode.BadRequest, $"The profile '{id}' does not exist.");
                }
            }
        }


    }
}