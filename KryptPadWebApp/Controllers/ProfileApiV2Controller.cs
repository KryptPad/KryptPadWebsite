using KryptPadWebApp.Models;
using KryptPadWebApp.Models.Results;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace KryptPadWebApp.Controllers
{
    [RoutePrefix("api/v2/profiles/{id}")]
    public class ProfileApiV2Controller : AuthorizedApiController
    {
        /// <summary>
        /// Gets the favorites for a profile
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("favorites")]
        public async Task<IHttpActionResult> GetFavorites(int id)
        {
            using (var ctx = new ApplicationDbContext())
            {
                var favorites = await (from i in ctx.Items
                                       join f in ctx.Favorites on new { i.Id } equals new { f.Item.Id }
                                       where i.Category.Profile.Id == id
                                       && i.Category.Profile.User.Id == UserId
                                       select i
                                 ).ToArrayAsync();
                    
                    
                // Return items
                return Json(new ItemsResult(favorites, Passphrase));
            }
        }

    }
}