using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
        //[Index("IX_Authorized", 1)]
        public ApplicationUser User { get; set; }

        /// <summary>
        /// Gets or sets the Id
        /// </summary>
        [Required]
        //[Index("IX_Authorized", 2)]
        public Guid AppId { get; set; }

        /// <summary>
        /// Gets or sets the IP address from where the login attempt was made
        /// </summary>
        public string AccessedFromIPAddress { get; set; }

        /// <summary>
        /// Gets or sets the date when the user authorized the device
        /// </summary>
        [Required]
        public DateTime DateAuthorized { get; set; }

    }
}