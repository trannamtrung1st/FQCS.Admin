﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace FQCS.Admin.Business
{
    public class Settings
    {
        public string Name { get; set; }

        private static Settings _instance;
        public static Settings Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Settings();
                return _instance;
            }
        }
    }

}
