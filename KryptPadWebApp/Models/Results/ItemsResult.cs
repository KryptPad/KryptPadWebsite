using KryptPadWebApp.Cryptography;
using KryptPadWebApp.Models.ApiEntities;
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
                         Name = Encryption.DecryptFromString(i.Name, passphrase),
                         Notes = Encryption.DecryptFromString(i.Notes, passphrase),
                         Fields = (from f in i.Fields
                                   select new ApiField() {
                                       Id = f.Id,
                                       FieldType=f.FieldType,
                                       Name = Encryption.DecryptFromString(f.Name, passphrase),
                                       Value = Encryption.DecryptFromString(f.Value, passphrase)
                                   }).ToArray()
                     }).ToArray();
            
        }
    }
}