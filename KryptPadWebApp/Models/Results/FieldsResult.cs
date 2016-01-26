using KryptPadWebApp.Models.ApiEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KryptPadWebApp.Models.Results
{
    public class FieldsResult
    {
        public ApiField[] Fields { get; protected set; }

        public FieldsResult() { }
        public FieldsResult(ApiField[] fields, string passphrase)
        {

            foreach (var field in fields)
            {
                // Decrypt the data
                field.Name = Encryption.DecryptFromString(field.Name, passphrase);

            }

            Fields = fields;
        }
    }
}