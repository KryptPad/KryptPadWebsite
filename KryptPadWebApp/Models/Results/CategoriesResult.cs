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
        public CategoriesResult(Category[] categories, string passphrase)
        {
            Categories = (from c in categories
                          select new ApiCategory()
                          {
                              Id = c.Id,
                              Name = Encryption.DecryptFromString(c.Name, passphrase),
                              Items = (c.Items != null ? (from i in c.Items
                                                          select new ApiItem()
                                                          {
                                                              Id = i.Id,
                                                              Name = Encryption.DecryptFromString(i.Name, passphrase)
                                                          }).ToArray() : null)
                          }).ToArray();

        }

    }
}