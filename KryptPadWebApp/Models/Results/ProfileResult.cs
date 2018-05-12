using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KryptPadWebApp.Models.Results
{
    public class ProfileResult
    {
        public Profile[] Profiles { get; set; }

        public ProfileResult() { }
        public ProfileResult(Profile[] profiles)
        {

            Profiles = profiles;

        }
    }


}