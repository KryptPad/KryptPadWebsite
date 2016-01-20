using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KryptPadWebApp.Models
{
    public class Item
    {
        public int Id { get; set; }
        public int ItemType { get; set; }

        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the notes for the item
        /// </summary>
        public string Notes { get; set; }

        [Required]
        public Category Category { get; set; }

        
        public List<Field> Fields { get; set; }
    }
}