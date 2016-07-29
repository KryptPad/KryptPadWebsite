using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KryptPadWebApp.Models.Entities
{
    public class RefreshToken
    {
        /// <summary>
        /// Gets or sets the hash of the token
        /// </summary>
        [Key]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the user id associated with this refresh token
        /// </summary>
        [Required]
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the client id of the app
        /// </summary>
        [Required]
        public string ClientId { get; set; }

        /// <summary>
        /// Gets or sets the time the refresh token was issued
        /// </summary>
        [Required]
        public DateTime IssuedUtc { get; set; }

        /// <summary>
        /// Gets or sets the time the refresh token expires
        /// </summary>
        [Required]
        public DateTime ExpiresUtc { get; set; }

        /// <summary>
        /// Gets or sets the serialized ticket info
        /// </summary>
        [Required]
        public string Ticket { get; set; }

    }
}