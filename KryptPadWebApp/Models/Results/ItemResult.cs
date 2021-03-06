﻿using KryptPadWebApp.Cryptography;
using KryptPadWebApp.Models.ApiEntities;
using KryptPadWebApp.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KryptPadWebApp.Models.Results
{
    public class ItemResult
    {
        public ApiItem[] Items { get; protected set; }

        public ItemResult() { }
        public ItemResult(Item[] items, string passphrase) {

            Items = (from i in items
                     select new ApiItem()
                     {
                         Id = i.Id,
                         CategoryId = i.Category.Id,
                         BackgroundColor = i.BackgroundColor,
                         Name = Encryption.DecryptFromString(i.Name, passphrase),
                         Notes = Encryption.DecryptFromString(i.Notes, passphrase),
                         IsFavorite = i.IsFavorite,
                         Fields = (from f in i.Fields
                                   select new ApiField()
                                   {
                                       Id = f.Id,
                                       FieldType = f.FieldType,
                                       Name = Encryption.DecryptFromString(f.Name, passphrase),
                                       Value = Encryption.DecryptFromString(f.Value, passphrase)
                                   }).ToArray()
                     }).ToArray();

        }
    }
}