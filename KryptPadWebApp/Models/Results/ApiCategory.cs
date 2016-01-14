using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KryptPadWebApp.Models.Results
{
    public class ApiCategory
    {
        public string Name { get; set; }
        public ItemResult[] Items { get; set; }
    }
}