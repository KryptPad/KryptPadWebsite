//using KryptPadWebApp.Cryptography;
using KryptPadWebApp.Models;
using KryptPadWebApp.Models.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace KryptPadWebApp.Controllers
{

    [RoutePrefix("Api/Profiles/{profileId}/Categories/{categoryId}/Items")]
    public class ItemApiController : AuthorizedApiController
    {
        // GET api/<controller>
        [Route("")]
        public IHttpActionResult Get(int profileId, int categoryId)
        {
            using (var ctx = new ApplicationDbContext())
            {
                var items = (from i in ctx.Items
                             where i.Category.Id == categoryId &&
                                i.Category.Profile.Id == profileId &&
                                i.Category.Profile.User.Id == UserId
                             select i).ToArray();

                // Return items
                return Json(new ItemResult() { Items = items });
            }
        }

        // GET api/<controller>/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        [Route("")]
        public async Task<IHttpActionResult> Post(int profileId, int categoryId, [FromBody] Item item)
        {
            // Ignore the category field
            ModelState.Remove("item.Category");

            if (ModelState.IsValid)
            {
                using (var ctx = new ApplicationDbContext())
                {
                    // Get the category
                    var category = (from c in ctx.Categories
                                    where c.Id == categoryId &&
                                        c.Profile.Id == profileId &&
                                        c.Profile.User.Id == UserId
                                    select c).SingleOrDefault();

                    // Check to see if we have a category
                    if (category == null)
                    {
                        // No category found
                        return BadRequest("The specified category does not exist.");
                    }
                    // Add item to the category
                    item.Category = category;
                    // Encrypt name
                    item.Name = Convert.ToBase64String(Encryption.Encrypt(item.Name, "123"));
                    
                    // Add item to the items table
                    ctx.Items.Add(item);

                    // Save the changes
                    await ctx.SaveChangesAsync();

                    // OK
                    return Ok();

                }
            }
            else
            {
                return BadRequest(ModelState);
            }

        }

        // PUT api/<controller>/5
        [Route("{id}")]
        public async Task<IHttpActionResult> Put(int profileId, int categoryId, int id, [FromBody]Item item)
        {
            // Ignore the category field
            ModelState.Remove("item.Category");

            if (ModelState.IsValid)
            {
                using (var ctx = new ApplicationDbContext())
                {
                    // Get the item from the db
                    var obj = (from i in ctx.Items
                               where item.Id == id &&
                                item.Category.Profile.User.Id == UserId
                               select i).SingleOrDefault();

                    //ctx.Entry(item).State = System.Data.Entity.EntityState.Modified
                    obj.Name = item.Name;
                    obj.ItemType = item.ItemType;

                    // Save the changes
                    await ctx.SaveChangesAsync();

                    // OK
                    return Ok();

                }
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}