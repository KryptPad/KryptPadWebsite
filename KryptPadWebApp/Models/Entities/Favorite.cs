using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KryptPadWebApp.Models.Entities
{
    public class Favorite
    {

        /// <summary>
        /// Gets or sets the Id of the favorite
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Link back to item
        /// </summary>
        [JsonIgnore]
        [Required]
        public Item Item { get; set; }
    }
}