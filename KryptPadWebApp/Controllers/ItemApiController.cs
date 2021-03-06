﻿
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

    [RoutePrefix("Api/Profiles/{profileId}/Categories/{categoryId}/Items")]
    public class ItemApiController : AuthorizedApiController
    {

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

            using (var ctx = new ApplicationDbContext())
            {
                var items = (from i in ctx.Items
                             where i.Id == id &&
                                i.Category.Id == categoryId &&
                                i.Category.Profile.Id == profileId &&
                                i.Category.Profile.User.Id == UserId
                             select new
                             {
                                 Item = i,
                                 i.Fields,
                                 i.Category,
                                 // Determine if this item is in our list of favorites
                                 IsFavorite = (from f in ctx.Favorites
                                               where f.Item.Id == i.Id
                                               select true).FirstOrDefault()
                             });

                // Convert the list of items and items to the response we want to send back
                // We're doing this to explicitly set whether an item is a favorite or not
                foreach (var i in items)
                {
                    i.Item.IsFavorite = i.IsFavorite;
                    // This is just stupid, but EF has issues with including navigation properties
                    // when using projection :/
                    i.Item.Fields = i.Fields;
                }

                // Return items
                return Json(new ItemResult(items.Select(x => x.Item).ToArray(), Passphrase));
            }
        }

        // POST api/<controller>
        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> Post(int profileId, int categoryId, [FromBody] ApiItem request)
        {

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
                    // Encrypt data
                    item.Name = Encryption.EncryptToString(request.Name, Passphrase);
                    item.Notes = Encryption.EncryptToString(request.Notes, Passphrase);

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

            if (ModelState.IsValid)
            {
                using (var ctx = new ApplicationDbContext())
                {
                    // Get the item from the db
                    var item = (from i in ctx.Items.Include(x => x.Category)
                                where i.Id == id &&
                                    i.Category.Id == categoryId &&
                                    i.Category.Profile.Id == profileId &&
                                    i.Category.Profile.User.Id == UserId
                                select i).SingleOrDefault();

                    if (item != null)
                    {
                        // Encrypt data
                        item.Name = Encryption.EncryptToString(request.Name, Passphrase);
                        item.Notes = Encryption.EncryptToString(request.Notes, Passphrase);
                        item.BackgroundColor = request.BackgroundColor;

                        // Look for changes in categories
                        if (request.CategoryId != categoryId)
                        {
                            // Get new category
                            item.Category = await GetCategory(ctx, profileId, request.CategoryId);
                        }


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
        public async Task<IHttpActionResult> Delete(int profileId, int categoryId, int id)
        {

            // Find the item by the id and delete it
            using (var ctx = new ApplicationDbContext())
            {
                // Get the item from the db
                var obj = (from i in ctx.Items
                           where i.Id == id &&
                                i.Category.Id == categoryId &&
                                i.Category.Profile.Id == profileId &&
                                i.Category.Profile.User.Id == UserId
                           select i).SingleOrDefault();

                // Delete item
                ctx.Items.Remove(obj);

                // Save the changes
                await ctx.SaveChangesAsync();

                // OK
                return Ok();

            }
        }


        #region HelperMethods

        /// <summary>
        /// Gets a category by its id
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="profileId"></param>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        private async Task<Category> GetCategory(ApplicationDbContext ctx, int profileId, int categoryId) =>
            await (from c in ctx.Categories
                   where c.Id == categoryId
                      && c.Profile.Id == profileId
                      && c.Profile.User.Id == UserId
                   select c).SingleOrDefaultAsync();

        #endregion

    }
}