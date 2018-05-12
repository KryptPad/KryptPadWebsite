using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KryptPadWebApp.Models.Requests
{
    public class ChangePassphraseRequest
    {
        [Required]
        public string NewPassphrase { get; set; }
    }
}