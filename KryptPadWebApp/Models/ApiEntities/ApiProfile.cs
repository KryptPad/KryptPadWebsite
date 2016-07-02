using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KryptPadWebApp.Models.ApiEntities
{
    public class ApiProfile
    {
        /// <summary>
        /// Gets or sets the profile id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the profile
        /// </summary>
        [Required]
        [RegularExpression(@"^([a-zA-Z0-9_\-\s\.]+)$", ErrorMessage = "Name can only contain numbers, letters, spaces, and underscores")]
        public string Name { get; set; }
        
    }
}