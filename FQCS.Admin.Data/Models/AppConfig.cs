﻿using System;
using System.Collections.Generic;
using System.Text;

namespace FQCS.Admin.Data.Models
{
    public class AppConfig
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public bool IsDefault { get; set; }
    }
}
