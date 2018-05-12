using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KryptPadWebApp.Models.Results
{
    public class AccountDetailsResult
    {
        /// <summary>
        /// Gets or sets whether the email address is confirmed
        /// </summary>
        public bool EmailConfirmed { get; set; }

        /// <summary>
        /// Gets or sets the hash of the user's email address
        /// </summary>
        public string EmailHash { get; set; }
    }
}