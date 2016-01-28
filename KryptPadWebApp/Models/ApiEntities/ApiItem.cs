using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KryptPadWebApp.Models.ApiEntities
{
    public class ApiItem
    {
        /// <summary>
        /// Gets or sets the ID of the item
        /// </summary>
        public int Id { get; set; }

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
        /// Gets or set the fields for the item
        /// </summary>
        public ApiField[] Fields { get; set; }
    }
}