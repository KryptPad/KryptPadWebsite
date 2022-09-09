using KryptPadWebApp.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KryptPadWebApp.Models.Results
{
    public class ProfilesResult
    {
        public Profile[] Profiles { get; set; }

        public ProfilesResult() { }
        public ProfilesResult(Profile[] profiles)
        {

            Profiles = profiles;

        }
    }


}