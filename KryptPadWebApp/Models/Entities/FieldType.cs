using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KryptPadWebApp.Models
{
    public class FieldType
    {
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the field type
        /// </summary>
        [Required]
        public string Name { get; set; }
       
    }
}