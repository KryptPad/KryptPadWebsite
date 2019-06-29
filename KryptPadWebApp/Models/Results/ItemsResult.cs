using KryptPadWebApp.Cryptography;
using KryptPadWebApp.Models.ApiEntities;
using KryptPadWebApp.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KryptPadWebApp.Models.Results
{
    public class ItemsResult
    {
        public ApiItem[] Items { get; protected set; }

        public ItemsResult() { }
        public ItemsResult(Item[] items, string passphrase) {

            Items = (from i in items
                     select new ApiItem()
                     {
                         Id = i.Id,
                         CategoryId = i.Category.Id,
                         BackgroundColor = i.BackgroundColor,
                         Name = Encryption.DecryptFromString(i.Name, passphrase),
                         IsFavorite = i.IsFavorite
                     }).ToArray();

        }
    }
}