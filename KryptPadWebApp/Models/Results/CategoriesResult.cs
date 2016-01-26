using KryptPadWebApp.Models.ApiEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KryptPadWebApp.Models.Results
{
    public class CategoriesResult
    {

        public ApiCategory[] Categories { get; set; }

        public CategoriesResult() { }
        public CategoriesResult(ApiCategory[] categories, string passphrase)
        {
            Categories = categories;
        }

    }
}