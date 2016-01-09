using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KryptPadWebApp.Models.Results
{
    public class ItemResult
    {
        public Item[] Items { get; protected set; }

        public ItemResult() { }
        public ItemResult(Item[] items, string passphrase) {

            foreach (var item in items)
            {
                // Decrypt the data
                item.Name = Encryption.DecryptFromString(item.Name, passphrase);

            }

            Items = items;
        }
    }
}