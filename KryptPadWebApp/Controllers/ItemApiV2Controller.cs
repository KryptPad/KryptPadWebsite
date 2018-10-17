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
using KryptPadWebApp.Cryptography;
using KryptPadWebApp.Models.Entities;

namespace KryptPadWebApp.Controllers
{

    [RoutePrefix("api/v2/items/{id}")]
    public class ItemApiV2Controller : AuthorizedApiController
    {

        // POST api/<controller>/5
        [HttpPost]
        [Route("set-fav")]
        public async Task<IHttpActionResult> SetFavorite(int id)
        {
            // Add the item to the favorites
            using (var ctx = new ApplicationDbContext())
            {
                // Get the item by its ID
                var item = await (from t in ctx.Items
                                  where t.Id == id && t.Category.Profile.User.Id == UserId
                                  select t).FirstOrDefaultAsync();

                if (item != null)
                {
                    var fav = new Favorite();
                    fav.Item = item;

                    // Add to model
                    ctx.Favorites.Add(fav);

                    // Save the fav
                    await ctx.SaveChangesAsync();

                    return Ok();
                }
                else
                {
                    // Something isn't right. Either the id is invalid or belongs to someone else
                    return BadRequest();
                }
                                               
            }


        }
    }

}
