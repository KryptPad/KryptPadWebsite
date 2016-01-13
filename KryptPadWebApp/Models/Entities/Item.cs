using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KryptPadWebApp.Models
{
    public class Item
    {
        public int Id { get; set; }
        public int ItemType { get; set; }
        [Required]
        public string Name { get; set; }

        [Required]
        [JsonIgnore] //prevent self referencing nightmare
        public Category Category { get; set; }

    }
}