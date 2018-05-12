using KryptPadWebApp.Models;
using KryptPadWebApp.Models.ApiEntities;
using KryptPadWebApp.Models.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Data.Entity;
using KryptPadWebApp.Cryptography;

namespace KryptPadWebApp.Controllers
{
    [RoutePrefix("Api/Profiles/{profileId}/Categories/{categoryId}/Items/{itemId}/Fields")]
    public class FieldApiController : AuthorizedApiController
    {
        /// <summary>
        /// Gets the fields from the specified item
        /// </summary>
        /// <param name="profileId"></param>
        /// <param name="categoryId"></param>
        /// <param name="itemId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        public IHttpActionResult Get(int profileId, int categoryId, int itemId)
        {
            
            using (var ctx = new ApplicationDbContext())
            {
                var fields = (from f in ctx.Fields
                              where f.Item.Id == itemId &&
                                 f.Item.Category.Id == categoryId &&
                                 f.Item.Category.Profile.Id == profileId &&
                                 f.Item.Category.Profile.User.Id == UserId
                              select new ApiField
                              {
                                  Id = f.Id,
                                  Name = f.Name,
                                  Value = f.Value
                              }).ToArray();

                // Return items
                return Json(new FieldsResult(fields, Passphrase));
            }
        }

        /// <summary>
        /// Posts a new field to the item
        /// </summary>
        /// <param name="profileId"></param>
        /// <param name="categoryId"></param>
        /// <param name="itemId"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> Post(int profileId, int categoryId, int itemId, [FromBody]ApiField request)
        {
            
            if (ModelState.IsValid)
            {
                using (var ctx = new ApplicationDbContext())
                {
                    // Get the item
                    var item = (from i in ctx.Items
                                where i.Id == itemId &&
                                    i.Category.Id == categoryId &&
                                    i.Category.Profile.Id == profileId &&
                                    i.Category.Profile.User.Id == UserId
                                select i).SingleOrDefault();

                    // Check for the item
                    if (item == null)
                    {
                        return BadRequest("The specified item does not exist.");
                    }

                    // Create field object
                    var field = new Field();

                    // Encrypt name and value
                    field.Name = Encryption.EncryptToString(request.Name, Passphrase);
                    field.Value = Encryption.EncryptToString(request.Value, Passphrase);

                    // Set other values
                    field.FieldType = request.FieldType;
                    field.Item = item;

                    // Add field to the DB
                    ctx.Fields.Add(field);

                    // Save to database
                    await ctx.SaveChangesAsync();

                    // Return created
                    return Ok(field.Id);

                }

            }
            else
            {
                // Opps
                return BadRequest(ModelState);
            }
        }

        /// <summary>
        /// Updates a specified field
        /// </summary>
        /// <param name="profileId"></param>
        /// <param name="categoryId"></param>
        /// <param name="itemId"></param>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("{id}")]
        public async Task<IHttpActionResult> Put(int profileId, int categoryId, int itemId, int id, [FromBody]ApiField request)
        {

            if (ModelState.IsValid)
            {
                using (var ctx = new ApplicationDbContext())
                {

                    // Get the field
                    var field = (from f in ctx.Fields.Include(x => x.Item)
                                 where f.Id == id &&
                                    f.Item.Id == itemId &&
                                    f.Item.Category.Id == categoryId &&
                                    f.Item.Category.Profile.Id == profileId &&
                                    f.Item.Category.Profile.User.Id == UserId
                                 select f).SingleOrDefault();

                    // Check to make sure we have a field
                    if (field == null)
                    {
                        return BadRequest("The specified field does not exist.");
                    }

                    // Encrypt name and value
                    field.Name = Encryption.EncryptToString(request.Name, Passphrase);
                    field.Value = Encryption.EncryptToString(request.Value, Passphrase);

                    // Save to database
                    await ctx.SaveChangesAsync();

                    // Return created
                    return Ok(field.Id);

                }

            }
            else
            {
                // Opps
                return BadRequest(ModelState);
            }
        }


        /// <summary>
        /// Deletes a field from the system
        /// </summary>
        /// <param name="profileId"></param>
        /// <param name="categoryId"></param>
        /// <param name="itemId"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{id}")]
        public async Task<IHttpActionResult> Delete(int profileId, int categoryId, int itemId, int id)
        {
            using (var ctx = new ApplicationDbContext())
            {
                // Find the field and delete it
                var field = (from f in ctx.Fields
                             where f.Id == id &&
                                f.Item.Id == itemId &&
                                f.Item.Category.Id == categoryId &&
                                f.Item.Category.Profile.Id == profileId &&
                                f.Item.Category.Profile.User.Id == UserId
                             select f).SingleOrDefault();

                if (field != null)
                {
                    // Remove field
                    ctx.Fields.Remove(field);
                    // Save
                    await ctx.SaveChangesAsync();

                    return Ok();
                }
                else
                {
                    return BadRequest("The specified field does not exist.");
                }

            }
        }
    }
}
