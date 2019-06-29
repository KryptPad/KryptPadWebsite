using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KryptPadWebApp.Models.Entities
{
    public class Category
    {
        /// <summary>
        /// Gets or sets the Id of the category
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the category name
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Link back to the profile
        /// </summary>
        [JsonIgnore]
        [Required]
        public Profile Profile { get; set; }

        /// <summary>
        /// List of items under the category
        /// </summary>
        public List<Item> Items { get; set; }
    }
}