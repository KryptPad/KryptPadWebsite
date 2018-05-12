using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KryptPadWebApp.Models.Entities
{
    public class AuthorizedClients
    {
        
        /// <summary>
        /// Gets or sets the id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the application name
        /// </summary>
        [Required]
        public string AppName { get; set; }

        /// <summary>
        /// Gets or sets the client id of the app
        /// </summary>
        [Required]
        public string ClientId { get; set; }

        /// <summary>
        /// Gets or sets the client secret
        /// </summary>
        [Required]
        public string ClientSecret { get; set; }

    }
}