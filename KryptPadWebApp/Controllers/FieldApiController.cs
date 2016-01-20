using KryptPadWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace KryptPadWebApp.Controllers
{
    [RoutePrefix("Api/Profiles/{profileId}/Categories/{categoryId}/Items/{itemId}/Fields")]
    public class FieldApiController : AuthorizedApiController
    {

        [Route("")]
        public IHttpActionResult Get(int profileId, int categoryId)
        {
            // Get the passphrase from the header
            var passphrase = Request.Headers.GetValues("Passphrase").First();

            using (var ctx = new ApplicationDbContext())
            {
                var items = (from i in ctx.Fields
                             where i.Category.Id == categoryId &&
                                i.Category.Profile.Id == profileId &&
                                i.Category.Profile.User.Id == UserId
                             select new ItemResult
                             {
                                 Name = i.Name
                             }).ToArray();

                // Return items
                return Json(new ItemsResult(items, passphrase));
            }
        }

    }
}
