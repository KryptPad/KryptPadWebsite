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
        public string Name { get; set; }
        
    }
}