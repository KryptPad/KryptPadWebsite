using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace KryptPadWebApp.Models
{
    public class HomeIndexViewModel
    {
        #region Properties
        
        public SigninModel SigninModel { get; set; } = new SigninModel();

        #endregion

        public HomeIndexViewModel()
        {
            
        }
    }
}