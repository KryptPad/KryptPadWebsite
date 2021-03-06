﻿using KryptPadWebApp.Cryptography;
using KryptPadWebApp.Models.ApiEntities;
using KryptPadWebApp.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KryptPadWebApp.Models.Results
{
    public class ItemSearchResult
    {
        public ApiCategory[] Categories { get; set; }

        public ItemSearchResult() { }
        public ItemSearchResult(Category[] categories, string passphrase)
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
                                                              Notes = Encryption.DecryptFromString(i.Notes, passphrase),
                                                              Fields = (i.Fields != null ? (from f in i.Fields
                                                                                            select new ApiField(f, passphrase)).ToArray() : null)
                                                          }).ToArray() : null)
                          }).ToArray();

        }
    }
}