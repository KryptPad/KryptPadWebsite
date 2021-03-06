﻿using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KryptPadWebApp.Models.Entities
{
    public class Item
    {
        /// <summary>
        /// Gets or sets the Id of the item
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the item type
        /// </summary>
        public int ItemType { get; set; }

        /// <summary>
        /// Gets or sets the name of the item
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the notes for the item
        /// </summary>
        public string Notes { get; set; }

        /// <summary>
        /// Link back to the category
        /// </summary>
        [JsonIgnore]
        [Required]
        public Category Category { get; set; }

        /// <summary>
        /// Gets or sets the item's background color
        /// </summary>
        public string BackgroundColor { get; set; }

        /// <summary>
        /// Gets or sets the item's icon
        /// </summary>
        public char Icon { get; set; }

        /// <summary>
        /// List of fields under the item
        /// </summary>
        public List<Field> Fields { get; set; }
               
        [NotMapped]
        public bool IsFavorite { get; set; }


    }
}