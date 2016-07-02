using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KryptPadWebApp.Models.ApiEntities
{
    public class ApiField
    {
        /// <summary>
        /// Gets or sets the ID of the field
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the field type
        /// </summary>
        [Required]
        public int FieldType { get; set; }

        /// <summary>
        /// Gets or sets the name of the field. e.g Password
        /// </summary>
        [Required]
        [RegularExpression(@"^([a-zA-Z0-9_\-\s\.]+)$", ErrorMessage = "Name can only contain numbers, letters, spaces, and underscores")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the Base64 encoded cipher text
        /// </summary>
        public string Value { get; set; }
    }
}