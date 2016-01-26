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
            // Get the passphrase from the header
            var passphrase = Request.Headers.GetValues("Passphrase").First();

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
                                  Name = f.Name
                              }).ToArray();

                // Return items
                return Json(new FieldsResult(fields, passphrase));
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
            // Get the passphrase from the header
            var passphrase = Request.Headers.GetValues("Passphrase").First();

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
                    field.Name = Encryption.EncryptToString(request.Name, passphrase);
                    field.Value = Encryption.EncryptToString(request.Value, passphrase);

                    // Set item
                    field.Item = item;

                    // Add field to the DB
                    ctx.Fields.Add(field);

                    // Save to database
                    await ctx.SaveChangesAsync();

                    // Return created
                    return Ok();

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
        [Route("")]
        public async Task<IHttpActionResult> Delete(int profileId, int categoryId, int itemId, int id)
        {
            using (var ctx = new ApplicationDbContext())
            {
                // Find the field and delete it
                var field = (from f in ctx.Fields
                             where f.Item.Id == itemId &&
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
