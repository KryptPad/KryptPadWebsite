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
                                       Name = Encryption.DecryptFromString(f.Name, passphrase)
                                   }).ToArray()
                     }).ToArray();
            

            //var apiItems = new List<ApiItem>();

            //foreach (var item in items)
            //{
            //    var apiItem = new ApiItem();

            //    // Set properties
            //    apiItem.Id = item.Id;
                
            //    // Decrypt the data
            //    apiItem.Name = Encryption.DecryptFromString(item.Name, passphrase);
            //    apiItem.Notes = Encryption.DecryptFromString(item.Notes, passphrase);

            //    if (item.Fields != null)
            //    {

            //        // Create a field list
            //        var apiFields = new List<ApiField>();

            //        foreach (var field in item.Fields)
            //        {
            //            // Create field
            //            var apiField = new ApiField();

            //            // Decrypt the data
            //            apiField.Name = Encryption.DecryptFromString(field.Name, passphrase);

            //            // Add field to list
            //            apiFields.Add(apiField);
            //        }

            //        // Add the fields to the item
            //        apiItem.Fields = apiFields.ToArray();
            //    }

            //    // Add the ApiItem to the list
            //    apiItems.Add(apiItem);
            //}

            //Items = apiItems.ToArray();
        }
    }
}