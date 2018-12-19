using KryptPadWebApp.Cryptography;
using KryptPadWebApp.Models;
using KryptPadWebApp.Models.ApiEntities;
using KryptPadWebApp.Models.Entities;
using KryptPadWebApp.Models.Results;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace KryptPadWebApp.Controllers
{
    [RoutePrefix("Api/Profiles/{profileId}/Categories")]
    public class CategoryApiController : AuthorizedApiController
    {

        [HttpGet]
        [Route("with-items", Name = "ProfileCategoriesWithItems")]
        public IHttpActionResult GetWithItem(int profileId)
        {

            using (var ctx = new ApplicationDbContext())
            {
                var categories = (from c in ctx.Categories.Include((cat) => cat.Items)
                                  where c.Profile.User.Id == UserId &&
                                  c.Profile.Id == profileId
                                  select new
                                  {
                                      Category = c,
                                      Items = (from i in c.Items
                                               select new
                                               {
                                                   Item = i,
                                                   IsFavorite = (from f in ctx.Favorites
                                                                 where f.Item.Id == i.Id
                                                                 select true).FirstOrDefault()
                                               })
                                  });

                // Convert the list of categories and items to the response we want to send back
                // We're doing this to explicitly set whether an item is a favorite or not
                foreach (var c in categories)
                {
                    foreach (var i in c.Items)
                    {
                        var item = c.Category.Items.Where(x => x.Id == i.Item.Id).First();
                        item.IsFavorite = i.IsFavorite;
                    }
                }

                // Convert to resultset we want
                return Json(new CategoriesResult(categories.Select(x => x.Category).ToArray(), Passphrase));
            }


        }

        // GET: CategoryApi
        [HttpGet]
        [Route("", Name = "ProfileCategories")]
        public IHttpActionResult Get(int profileId)
        {

            using (var ctx = new ApplicationDbContext())
            {
                var categories = (from c in ctx.Categories
                                  where c.Profile.User.Id == UserId &&
                                  c.Profile.Id == profileId
                                  select c).ToArray();


                return Json(new CategoriesResult(categories, Passphrase));
            }


        }

        // POST: CategoryApi
        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> Post(int profileId, [FromBody]ApiCategory request)
        {
            if (ModelState.IsValid)
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
                        return BadRequest("The specified profile does not exist.");
                    }

                    var category = new Category();

                    // Encrypt the category name
                    category.Name = Encryption.EncryptToString(request.Name, Passphrase);

                    // Set the profile to the category
                    category.Profile = profile;

                    // Add to the context
                    ctx.Categories.Add(category);
                    // Save changes
                    await ctx.SaveChangesAsync();

                    return Ok(category.Id);
                }
            }
            else
            {
                return BadRequest(ModelState);
            }


        }

        // PUT:
        [HttpPut]
        [Route("{id}")]
        public async Task<IHttpActionResult> Put(int profileId, int id, [FromBody]ApiCategory category)
        {
            if (ModelState.IsValid)
            {
                using (var ctx = new ApplicationDbContext())
                {

                    // Find the profile
                    var obj = (from c in ctx.Categories.Include((x) => x.Items).Include((x) => x.Profile)
                               where c.Id == id
                                   && c.Profile.Id == profileId
                                   && c.Profile.User.Id == UserId
                               select c).FirstOrDefault();


                    // Did we find one?
                    if (obj == null)
                    {
                        return BadRequest("The specified category does not exist.");
                    }

                    // Encrypt the category name
                    obj.Name = Encryption.EncryptToString(category.Name, Passphrase);

                    // Save changes
                    await ctx.SaveChangesAsync();

                    return Ok(obj.Id);
                }
            }
            else
            {
                return BadRequest(ModelState);
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
                    return BadRequest("The specified category does not exist.");
                }

                ctx.Categories.Remove(obj);

                // Save changes
                await ctx.SaveChangesAsync();

                return Ok();
            }
        }

    }
}