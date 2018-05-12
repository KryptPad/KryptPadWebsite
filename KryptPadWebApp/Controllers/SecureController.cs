using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KryptPadWebApp.Controllers
{
    // If debugging, we remove the RequireHttps attribute 
#if !DEBUG
    [RequireHttps]
#endif
    public class SecureController : Controller
    {
        
    }
}