using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KryptPadWebApp.Models
{
    public class Field
    {
        public int Id { get; set; }
        public int FieldType { get; set; }
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the Base64 encoded cipher text
        /// </summary>
        public string Value { get; set; }

        [Required]
        public Item Item { get; set; }
    }
}