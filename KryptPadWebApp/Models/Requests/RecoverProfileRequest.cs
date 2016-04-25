using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KryptPadWebApp.Models.Requests
{
    public class RecoverProfileRequest
    {
        /// <summary>
        /// Gets or sets the encryption key
        /// </summary>
        public byte[] Key { get; set; }

        /// <summary>
        /// Gets or sets the new passphrase for encryption
        /// </summary>
        public string NewPassphrase { get; set; }

    }
}