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
using System.Data.Entity;
using System.Transactions;
using Newtonsoft.Json.Linq;

namespace KryptPadWebApp.Controllers
{

    [RoutePrefix("Api/Profiles")]
    public class ProfileApiController : AuthorizedApiController
    {
        /// <summary>
        /// Gets a list of all the profiles for a user
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Gets the profile by its id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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

                    if (VerifyPassphrase(profile, Passphrase))
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

        /// <summary>
        /// Downloads the entire profile (encrypted) for use with KryptPad desktop or for backup purposes
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}/Download")]
        public async Task<IHttpActionResult> Download(int id)
        {

            // Get the specified profile
            using (var ctx = new ApplicationDbContext())
            {
                var profile = await (from p in ctx.Profiles
                                     where p.Id == id
                                         && p.User.Id == UserId
                                     select p).SingleOrDefaultAsync();

                // Check profile
                if (profile != null)
                {

                    if (VerifyPassphrase(profile, Passphrase))
                    {

                        // Download the entire profile
                        var profileToDownload = await (from p in ctx.Profiles
                                                        .Include(x => x.Categories.Select(y => y.Items.Select(z => z.Fields)))
                                                       where p.Id == id
                                                            && p.User.Id == UserId
                                                       select p).SingleAsync();
                        // Return the profile
                        return Json(profileToDownload);
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

        /// <summary>
        /// Creates a new profile with uploaded profile data
        /// </summary>
        /// <param name="profile"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Upload")]
        public async Task<IHttpActionResult> Upload(Profile profile)
        {
            using (var ctx = new ApplicationDbContext())
            {
                // Find the user
                var user = ctx.Users.Find(UserId);

                if (user == null)
                {
                    return BadRequest("User not found");
                }

                //// Look for a profile with the same name, if there are one or more, count them and add a (n) to the name
                //var dupes = (from p in ctx.Profiles
                //             where p.User.Id == user.Id
                //             && p.Name == profile.Name
                //             select p).Count();

                //// Add dupe count to profile name
                //if (dupes > 0)
                //{
                //    profile.Name = profile.Name + $" ({dupes})";
                //}

                // Create a profile
                profile.Id = 0;
                profile.User = user;

                // Add the profile to the context
                ctx.Profiles.Add(profile);

                // Save new profile
                await ctx.SaveChangesAsync();

                // Ok
                return Ok(profile.Id);
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
                        return BadRequest("User not found");
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

        /// <summary>
        /// Changes the passphrase
        /// </summary>
        /// <param name="id"></param>
        /// <param name="newPassphrase"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("{id}/Change-Passphrase")]
        public async Task<IHttpActionResult> ChangePassphrase(int id, [FromBody]string newPassphrase)
        {
            if (!string.IsNullOrWhiteSpace(newPassphrase))
            {
                // Update the profile with the new passphrase
                //using (var scope = new TransactionScope())
                using (var ctx = new ApplicationDbContext())
                {
                    // First, verify that the supplied passphrase is correct
                    var profile = (from p in ctx.Profiles
                                   where p.Id == id
                                       && p.User.Id == UserId
                                   select p).SingleOrDefault();

                    // Check profile
                    if (profile != null)
                    {

                        if (VerifyPassphrase(profile, Passphrase))
                        {
                            // Get all items in the profile
                            var categories = (from c in ctx.Categories.Include(c => c.Items.Select(y => y.Fields))
                                              where c.Profile.Id == id
                                                && c.Profile.User.Id == UserId
                                              select c);

                            // Passphrase is correct
                            foreach (var category in categories)
                            {
                                // Re-encrypt with new passphrase
                                category.Name = Encryption.ReEncryptToString(category.Name, Passphrase, newPassphrase);

                                // Re-encrypt items
                                foreach (var item in category.Items)
                                {
                                    // Re-encrypt with new passphrase
                                    item.Name = Encryption.ReEncryptToString(item.Name, Passphrase, newPassphrase);
                                    item.Notes = Encryption.ReEncryptToString(item.Notes, Passphrase, newPassphrase);

                                    // Now re-encrypt the fields
                                    foreach (var field in item.Fields)
                                    {
                                        // Re-encrypt with new passphrase
                                        field.Name = Encryption.ReEncryptToString(field.Name, Passphrase, newPassphrase);
                                        field.Value = Encryption.ReEncryptToString(field.Value, Passphrase, newPassphrase);
                                    }
                                }
                            }

                            // Generate a random salt for the profile
                            var saltBytes = Encryption.GenerateSalt();

                            // Hash passphrase with salt
                            profile.Key1 = Convert.ToBase64String(saltBytes);
                            profile.Key2 = Encryption.Hash(newPassphrase, saltBytes);

                            // Save the changes
                            await ctx.SaveChangesAsync();

                            // Done
                            return Ok();

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
            else
            {
                // Must have a value
                return BadRequest("Passphrase cannot be null or blank");
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
                    return BadRequest("The specified profile does not exist.");
                }
            }
        }

        /// <summary>
        /// Gets all items in a profile
        /// </summary>
        /// <param name="profileId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}/Items")]
        public IHttpActionResult GetItems(int id, string q)
        {
            using (var ctx = new ApplicationDbContext())
            {
                var categories = (from c in ctx.Categories.Include((cat) => cat.Items)
                                  where c.Profile.User.Id == UserId &&
                                  c.Profile.Id == id
                                  select c).ToArray();

                // Create a flat list of items
                var itemsToRemove = new List<Item>();
                var categoriesToRemove = new List<Category>();

                // Decrypt the text so we can search
                foreach (var category in categories)
                {
                    foreach (var item in category.Items)
                    {
                        item.Name = Encryption.DecryptFromString(item.Name, Passphrase);
                        // If this item does not match, remove it from the list
                        if (q != null && (item.Name.IndexOf(q, StringComparison.CurrentCultureIgnoreCase) < 0))
                        {
                            // Add to the remove list
                            itemsToRemove.Add(item);
                        }
                    }

                    // Remove any items from this category
                    foreach (var item in itemsToRemove)
                    {
                        category.Items.Remove(item);
                    }

                }

                // Remove any empty categories
                categories = (from c in categories
                              where c.Items.Any()
                              select c).ToArray();


                return Json(new ItemSearchResult(categories, Passphrase));
            }

        }

        #region Helper methods

        /// <summary>
        /// Verifies the supplied passphrase
        /// </summary>
        /// <param name="passphrase"></param>
        /// <returns></returns>
        private bool VerifyPassphrase(Profile profile, string passphrase)
        {
            // If there is no salt, set empty
            var salt = profile.Key1 ?? string.Empty;

            // Get the salt
            var saltBytes = Convert.FromBase64String(salt);

            // Verify the supplied passphrase
            var hashedPassphrase = Encryption.Hash(Passphrase, saltBytes);

            // Compare the hashes
            return hashedPassphrase.Equals(profile.Key2);
        }

        #endregion
    }
}