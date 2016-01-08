using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KryptPadWebApp.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }

        [Required]
        public Profile Profile { get; set; }

        public List<Item> Items { get; set; }
    }
}