﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gateway
{
    public class AppSettings
    {
        public string Secret { get; set; }

        public string[] AllowedHosts { get; set; }

        public string[] WhitelistedUrls { get; set; }
    }
}
