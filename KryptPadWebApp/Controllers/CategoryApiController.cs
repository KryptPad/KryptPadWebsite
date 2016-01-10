using KryptPadWebApp.Models;
using KryptPadWebApp.Models.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Data.Entity;

namespace KryptPadWebApp.Controllers
{
    [RoutePrefix("Api/Profiles/{profileId}/Categories")]
    public class CategoryApiController : AuthorizedApiController
    {
        // GET: CategoryApi
        [HttpGet]
        [Route("", Name = "ProfileCategories")]
        public IHttpActionResult Get(int profileId)
        {

            // Get the passphrase from the header
            var passphrase = Request.Headers.GetValues("Passphrase").First();

            using (var ctx = new ApplicationDbContext())
            {
                var categories = (from c in ctx.Categories.Include((cat) => cat.Items)
                                  where c.Profile.User.Id == UserId &&
                                  c.Profile.Id == profileId
                                  select c).ToArray();

                return Json(new CategoryResult(categories, passphrase));
            }


        }

        // POST: CategoryApi
        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> Post(int profileId, [FromBody]Category category)
        {

            // Get the passphrase from the header
            var passphrase = Request.Headers.GetValues("Passphrase").First();

            using (var ctx = new ApplicationDbContext())
            {

                // Find the profile
                var profile = (from p in ctx.Profiles
                               where p.User.Id == UserId &&
                               p.Id == profileId
                               select p).SingleOrDefault();

                // Did we find one?
                if (profile == null)
                {
                    return Content(HttpStatusCode.BadRequest, "Profile not found");
                }

                // Encrypt the category name
                category.Name = Encryption.EncryptToString(category.Name, passphrase);

                // Set the profile to the category
                category.Profile = profile;

                // Add to the context
                ctx.Categories.Add(category);
                // Save changes
                await ctx.SaveChangesAsync();

                return Json(new CategoryResult(new[] { category }, passphrase));
            }


        }

        // PUT:
        [HttpPut]
        [Route("{id}")]
        public async Task<IHttpActionResult> Put(int profileId, int id, [FromBody]Category category)
        {
            // Get the passphrase from the header
            var passphrase = Request.Headers.GetValues("Passphrase").First();

            using (var ctx = new ApplicationDbContext())
            {

                // Find the profile
                var obj = (from c in ctx.Categories
                           where c.Id == id
                               && c.Profile.Id == profileId
                               && c.Profile.User.Id == UserId
                           select c).FirstOrDefault();


                // Did we find one?
                if (obj == null)
                {
                    return Content(HttpStatusCode.BadRequest, "Category not found");
                }

                // Encrypt the category name
                obj.Name = Encryption.EncryptToString(category.Name, passphrase);
                
                // Save changes
                await ctx.SaveChangesAsync();

                return Ok();
            }
        }

        // DELETE:
        [HttpDelete]
        [Route("{id}")]
        public async Task<IHttpActionResult> Delete(int profileId, int id)
        {
            
            using (var ctx = new ApplicationDbContext())
            {

                // Find the profile
                var obj = (from c in ctx.Categories
                           where c.Id == id
                               && c.Profile.Id == profileId
                               && c.Profile.User.Id == UserId
                           select c).FirstOrDefault();


                // Did we find one?
                if (obj == null)
                {
                    return Content(HttpStatusCode.BadRequest, "Category not found");
                }

                ctx.Categories.Remove(obj);

                // Save changes
                await ctx.SaveChangesAsync();

                return Ok();
            }
        }

    }
}