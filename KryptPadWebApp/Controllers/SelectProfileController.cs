using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KryptPadWebApp.Controllers
{
    [Authorize]
    [RoutePrefix("App")]
    public class SelectProfileController : SecureController
    {
        // GET: SelectProfile
        [HttpGet]
        [Route("Select-Profile", Name = "SelectProfileRoute")]
        public ActionResult SelectProfile()
        {
            return View();
        }
    }
}