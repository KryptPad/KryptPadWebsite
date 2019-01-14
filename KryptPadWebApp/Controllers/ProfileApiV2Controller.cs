using KryptPadWebApp.Models;
using KryptPadWebApp.Models.Results;
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
                var favorites = await (from f in ctx.Favorites.Include(x => x.Item.Category)
                                       where f.Item.Category.Profile.Id == id
                                          && f.Item.Category.Profile.User.Id == UserId
                                       select f
                                 ).ToArrayAsync();

                // Return items
                return Json(new ItemsResult(favorites.Select(x => x.Item).ToArray(), Passphrase));
            }
        }

    }
}