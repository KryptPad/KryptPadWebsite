using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KryptPadWebApp.Models.ApiEntities
{
    public class ApiAccount
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}