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
        /// Gets or sets the Id
        /// </summary>
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the user
        /// </summary>
        [JsonIgnore]
        [Key]
        public ApplicationUser User { get; set; }


    }
}