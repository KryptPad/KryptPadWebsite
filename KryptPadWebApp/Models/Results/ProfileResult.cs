﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KryptPadWebApp.Models.Results
{
    public class ProfileResult
    {
        public Profile[] Profiles { get; set; }

        public ProfileResult() { }
        public ProfileResult(Profile[] profiles, string passphrase)
        {
            foreach (var profile in profiles)
            {
                // Decrypt the data, but in this case, we already have a decrypted form
                profile.Name = Encryption.DecryptFromString(profile.Name, passphrase);

            }

            Profiles = profiles;

        }
    }


}