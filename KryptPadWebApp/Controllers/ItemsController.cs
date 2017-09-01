using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace KryptPadWebApp.Controllers
{
    
    [Authorize]
    [RoutePrefix("App")]
    public class ItemsController : SecureController
    {
        // GET: Items
        [HttpGet]
        [Route("Items", Name = "Items")]
        public ActionResult Items()
        {
            return View();
        }
    }
}