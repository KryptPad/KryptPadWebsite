using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KryptPadWebApp.Models.Results
{
    public class CategoryResult
    {

        public Category[] Categories { get; set; }

        public CategoryResult() { }
        public CategoryResult(Category[] categories)
        {
            Categories = categories;
        }

    }
}