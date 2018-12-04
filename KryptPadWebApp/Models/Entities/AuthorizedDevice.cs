using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KryptPadWebApp.Models.Entities
{
    public class AuthorizedDevice
    {
        /// <summary>
        /// Primary key
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the user
        /// </summary>
        [JsonIgnore]
        [Required]
        public ApplicationUser User { get; set; }

        /// <summary>
        /// Gets or sets the Id
        /// </summary>
        [Required]
        public Guid AppId { get; set; }

    }
}