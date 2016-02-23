using KryptPadWebApp.Models;
using KryptPadWebApp.Models.ApiEntities;
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
            
            // Get the specified profile
            using (var ctx = new ApplicationDbContext())
            {
                var profile = (from p in ctx.Profiles
                               where p.Id == id
                                   && p.User.Id == UserId
                               select p).SingleOrDefault();

                // Check profile
                if (profile != null)
                {
                    // If there is no salt, set empty
                    var salt = profile.Key1 ?? string.Empty;

                    // Get the salt
                    var saltBytes = Convert.FromBase64String(salt);

                    // Verify the supplied passphrase
                    var hashedPassphrase = Encryption.Hash(Passphrase, saltBytes);

                    if (hashedPassphrase.Equals(profile.Key2))
                    {
                        // Return the profile
                        return Json(new ProfileResult(new[] { profile }));
                    }
                    else
                    {
                        // The passphrase is wrong, unauthorized
                        return Unauthorized();
                    }
                    
                }
                else
                {
                    // Record does not exist
                    return BadRequest("The specified profile does not exist");
                }
                
            }
        }

        // POST api/<controller>
        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> Post(ApiProfile request)
        {
            if (ModelState.IsValid)
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

                    // Generate a random salt for the profile
                    var saltBytes = Encryption.GenerateSalt();

                    // Create profile object to store in DB
                    var profile = new Profile()
                    {
                        User = user,
                        Name = request.Name,
                        Key1 = Convert.ToBase64String(saltBytes),
                        Key2 = Encryption.Hash(Passphrase, saltBytes)
                    };

                    // Add the profile to the context
                    ctx.Profiles.Add(profile);

                    // Save changes
                    await ctx.SaveChangesAsync();

                    // Ok
                    return Ok(profile.Id);
                }
            }
            else
            {
                return BadRequest(ModelState);
            }

        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IHttpActionResult> Put(int id, ApiProfile request)
        {

            if (ModelState.IsValid)
            {
                // Create context
                using (var ctx = new ApplicationDbContext())
                {
                    var profile = (from p in ctx.Profiles
                                   where p.User.Id == UserId
                                   && p.Id == id
                                   select p).SingleOrDefault();

                    // Update name
                    profile.Name = request.Name;

                    // Save changes
                    await ctx.SaveChangesAsync();

                    // Ok
                    return Ok(profile.Id);
                }
            }
            else
            {
                return BadRequest(ModelState);
            }

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

        /// <summary>
        /// Gets all items in a profile
        /// </summary>
        /// <param name="profileId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}/items")]
        public IHttpActionResult GetItems(int id)
        {

            using (var ctx = new ApplicationDbContext())
            {
                var items = (from i in ctx.Items
                             where i.Category.Profile.Id == id &&
                                i.Category.Profile.User.Id == UserId
                             select i).ToArray();

                // Return items
                return Json(new ItemsResult(items, Passphrase));
            }
        }

    }
}