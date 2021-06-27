﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PriSecFileStorageAPI
{
    public class LoginModels
    {
        public String RequestStatus { get; set; }

        public String UserIDChecker { get; set; }

        public String SignedRandomChallengeBase64String { get; set; }

        public String ServerECDSAPKBase64String { get; set; }
    }
}