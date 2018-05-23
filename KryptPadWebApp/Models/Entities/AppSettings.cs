using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KryptPadWebApp.Models
{
    public class AppSettings
    {
        public int Id { get; set; }
        public int Downloads { get; set; }
        public string BroadcastMessage { get; set; }
    }
}