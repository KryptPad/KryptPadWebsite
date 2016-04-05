using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KryptPadWebApp.Models.ApiEntities
{
    public class ApiField
    {
        /// <summary>
        /// Gets or sets the ID of the field
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the field type
        /// </summary>
        public int FieldType { get; set; }

        /// <summary>
        /// Gets or sets the name of the field. e.g Password
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the Base64 encoded cipher text
        /// </summary>
        public string Value { get; set; }
    }
}