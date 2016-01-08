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
            using (var ctx = new ApplicationDbContext())
            {
                var categories = (from c in ctx.Categories.Include((cat) => cat.Items)
                                  where c.Profile.User.Id == UserId &&
                                  c.Profile.Id == profileId
                                  select c).ToArray();

                return Json(new CategoryResult(categories));
            }


        }

        // POST: CategoryApi
        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> Post(int profileId, [FromBody]Category category)
        {
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

                // Set the profile to the category
                category.Profile = profile;

                // Add to the context
                ctx.Categories.Add(category);
                // Save changes
                await ctx.SaveChangesAsync();

                return Json(new CategoryResult(new[] { category }));
            }


        }
    }
}