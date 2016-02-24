using KryptPadWebApp.Models.ApiEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KryptPadWebApp.Models.Results
{
    public class ItemSearchResult
    {
        public ApiItem[] Items { get; protected set; }

        public ItemSearchResult() { }
        public ItemSearchResult(Item[] items, string passphrase)
        {

            var apiItems = new List<ApiItem>();

            foreach (var item in items)
            {
                var apiItem = new ApiItem();

                // Decrypt the data
                apiItem.Name = item.Name;
                apiItem.Notes = item.Notes;
                
                // Add the ApiItem to the list
                apiItems.Add(apiItem);
            }

            Items = apiItems.ToArray();
        }
    }
}