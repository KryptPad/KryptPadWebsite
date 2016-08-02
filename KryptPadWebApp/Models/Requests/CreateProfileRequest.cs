using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KryptPadWebApp.Models.Requests
{
    public class CreateProfileRequest
    {
        /// <summary>
        /// Gets or sets the name of the profile
        /// </summary>
        [Required]
        [RegularExpression(@"^([a-zA-Z0-9_\-\s\.]+)$", ErrorMessage = "Name can only contain numbers, letters, spaces, and underscores")]
        public string Name { get; set; }
        
        /// <summary>
        /// Gets or sets the passphrase for this profile
        /// </summary>
        [Required]
        public string Passphrase { get; set; }

        /// <summary>
        /// Gets or sets the confirm passphrase for this profile
        /// </summary>
        [Compare("Passphrase", ErrorMessage = "The passphrase and confirmation passphrase do not match.")]
        public string ConfirmPassphrase { get; set; }
    }
}