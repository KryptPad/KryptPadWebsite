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
        /// <summary>
        /// SelectProfile
        /// </summary>
        /// <returns></returns>
        [Route("Select-Profile", Name = "SelectProfile")]
        public ActionResult SelectProfile()
        {
            return View();
        }
    }
}