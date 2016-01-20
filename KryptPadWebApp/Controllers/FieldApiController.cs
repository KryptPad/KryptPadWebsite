using KryptPadWebApp.Models;
using KryptPadWebApp.Models.Results;
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

    }
}
