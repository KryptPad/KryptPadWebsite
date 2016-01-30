//using KryptPadWebApp.Cryptography;
using KryptPadWebApp.Models;
using KryptPadWebApp.Models.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using KryptPadWebApp.Models.ApiEntities;

namespace KryptPadWebApp.Controllers
{

    [RoutePrefix("Api/Profiles/{profileId}/Categories/{categoryId}/Items")]
    public class ItemApiController : AuthorizedApiController
    {
        /// <summary>
        /// Gets all items in the category
        /// </summary>
        /// <param name="profileId"></param>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        public IHttpActionResult Get(int profileId, int categoryId)
        {
            // Get the passphrase from the header
            var passphrase = Request.Headers.GetValues("Passphrase").First();

            using (var ctx = new ApplicationDbContext())
            {
                var items = (from i in ctx.Items
                             where i.Category.Id == categoryId &&
                                i.Category.Profile.Id == profileId &&
                                i.Category.Profile.User.Id == UserId
                             select new ApiItem
                             {
                                 Id = i.Id,
                                 Name = i.Name
                             }).ToArray();

                // Return items
                return Json(new ItemsResult(items, passphrase));
            }
        }

        /// <summary>
        /// Gets a specific item and details
        /// </summary>
        /// <param name="profileId"></param>
        /// <param name="categoryId"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}")]
        public IHttpActionResult Get(int profileId, int categoryId, int id)
        {
            // Get the passphrase from the header
            var passphrase = Request.Headers.GetValues("Passphrase").First();

            using (var ctx = new ApplicationDbContext())
            {
                var items = (from i in ctx.Items.Include(x => x.Fields)
                             where i.Id == id &&
                                i.Category.Id == categoryId &&
                                i.Category.Profile.Id == profileId &&
                                i.Category.Profile.User.Id == UserId
                             select i);

                var apiItems = new List<ApiItem>();

                // Convert to api response
                foreach (var item in items)
                {
                    var apiItem = new ApiItem
                    {
                        Id = item.Id,
                        Name = item.Name,
                        Fields = (from f in item.Fields
                                  select new ApiField
                                  {
                                      Id = f.Id,
                                      Name = f.Name
                                  }).ToArray()
                    };

                    apiItems.Add(apiItem);
                }

                // Return items
                return Json(new ItemsResult(apiItems.ToArray(), passphrase));
            }
        }

        // POST api/<controller>
        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> Post(int profileId, int categoryId, [FromBody] ApiItem request)
        {
            // Get the passphrase from the header
            var passphrase = Request.Headers.GetValues("Passphrase").First();

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

                    var item = new Item();

                    // Add item to the category
                    item.Category = category;
                    // Encrypt name
                    item.Name = Encryption.EncryptToString(request.Name, passphrase);

                    // Add item to the items table
                    ctx.Items.Add(item);

                    // Save the changes
                    await ctx.SaveChangesAsync();

                    // OK
                    return Ok(item.Id);

                }
            }
            else
            {
                return BadRequest(ModelState);
            }

        }

        // PUT api/<controller>/5
        [HttpPut]
        [Route("{id}")]
        public async Task<IHttpActionResult> Put(int profileId, int categoryId, int id, [FromBody]ApiItem request)
        {

            // Get the passphrase from the header
            var passphrase = Request.Headers.GetValues("Passphrase").First();

            if (ModelState.IsValid)
            {
                using (var ctx = new ApplicationDbContext())
                {
                    // Get the item from the db
                    var item = (from i in ctx.Items.Include(x => x.Category)
                                where i.Id == id && i.Category.Profile.User.Id == UserId
                                select i).SingleOrDefault();

                    if (item != null)
                    {
                        //ctx.Entry(item).State = System.Data.Entity.EntityState.Modified
                        item.Name = Encryption.EncryptToString(request.Name, passphrase);

                        // Save the changes
                        await ctx.SaveChangesAsync();

                        // OK
                        return Ok(item.Id);
                    }
                    else
                    {
                        return BadRequest("The specified item does not exist.");
                    }
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

            // Find the item by the id and delete it
            using (var ctx = new ApplicationDbContext())
            {
                // Get the item from the db
                var obj = (from i in ctx.Items
                           where i.Id == id && i.Category.Profile.User.Id == UserId
                           select i).SingleOrDefault();

                // Delete item
                ctx.Items.Remove(obj);

                // Save the changes
                await ctx.SaveChangesAsync();

                // OK
                return Ok();

            }
        }
    }
}