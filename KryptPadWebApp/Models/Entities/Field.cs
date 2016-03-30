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

        /// <summary>
        /// Gets or sets the name of the field. e.g Password
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the Base64 encoded cipher text
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets the field sort order
        /// </summary>
        public int SortOrder { get; set; }

        [Required]
        public Item Item { get; set; }
    }
}