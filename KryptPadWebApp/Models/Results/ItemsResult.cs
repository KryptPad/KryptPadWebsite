using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KryptPadWebApp.Models.Results
{
    public class ItemsResult
    {
        public ItemResult[] Items { get; protected set; }

        public ItemsResult() { }
        public ItemsResult(ItemResult[] items, string passphrase) {

            foreach (var item in items)
            {
                // Decrypt the data
                item.Name = Encryption.DecryptFromString(item.Name, passphrase);

            }

            Items = items;
        }
    }
}