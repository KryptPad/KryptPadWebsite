using KryptPadWebApp.Cryptography;
using KryptPadWebApp.Models.ApiEntities;
using KryptPadWebApp.Models.Entities;
using System.Linq;

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
                                                              CategoryId = c.Id,
                                                              Name = Encryption.DecryptFromString(i.Name, passphrase),
                                                              BackgroundColor = i.BackgroundColor,
                                                              IsFavorite = i.IsFavorite
                                                          }).ToArray() : null)
                          }).ToArray();

        }

    }
}