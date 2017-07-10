using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KryptPadWebApp.Controllers
{
    /// <summary>
    /// Forces the url to http. If in debug mode, this requirement is removed.
    /// </summary>
#if !DEBUG
    [RequireHttps]
#endif
    public class SecureController : Controller
    {
        
    }
}