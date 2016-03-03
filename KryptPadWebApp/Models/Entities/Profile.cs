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

        /// <summary>
        /// Gets or sets the Key1 value of the profile
        /// </summary>
        public string Key1 { get; set; }

        /// <summary>
        /// Gets or sets the Key2 value of the profile
        /// </summary>
        public string Key2 { get; set; }

        /// <summary>
        /// Navigate back to properties
        /// </summary>
        public List<Category> Categories { get; set; }
    }
}