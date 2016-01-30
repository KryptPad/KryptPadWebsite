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
        public ItemsResult(ApiItem[] items, string passphrase) {

            foreach (var item in items)
            {
                // Decrypt the data
                item.Name = Encryption.DecryptFromString(item.Name, passphrase);
                item.Notes = Encryption.DecryptFromString(item.Notes, passphrase);

                if (item.Fields != null)
                {
                    foreach (var field in item.Fields)
                    {
                        // Decrypt the data
                        field.Name = Encryption.DecryptFromString(field.Name, passphrase);
                        
                    }
                }
            }

            Items = items;
        }
    }
}