using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KryptPadWebApp.Models
{
    public class Profile
    {
        /// <summary>
        /// Gets or sets the Id of the profile
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Gets or sets the profile owner
        /// </summary>
        [JsonIgnore]
        public ApplicationUser User { get; set; }
        /// <summary>
        /// Gets or sets the name of the profile
        /// </summary>
        public string Name { get; set; }
        
    }
}