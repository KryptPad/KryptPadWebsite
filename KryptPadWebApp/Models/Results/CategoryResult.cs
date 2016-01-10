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
        public CategoryResult(Category[] categories, string passphrase)
        {

            foreach (var category in categories)
            {
                category.Name = Encryption.DecryptFromString(category.Name, passphrase);
            }

            Categories = categories;
        }

    }
}