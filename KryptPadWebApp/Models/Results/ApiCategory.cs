using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KryptPadWebApp.Models.Results
{
    public class ApiCategory
    {
        /// <summary>
        /// Gets or sets the ID of the category
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the category
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the list of items
        /// </summary>
        public ApiItem[] Items { get; set; }
    }
}