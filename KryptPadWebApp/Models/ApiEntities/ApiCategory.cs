using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KryptPadWebApp.Models.ApiEntities
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
        [Required]
        [RegularExpression(@"^([a-zA-Z0-9\s\W_]+)$", ErrorMessage = "Name contains invalid characters")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the list of items
        /// </summary>
        public ApiItem[] Items { get; set; }
    }
}