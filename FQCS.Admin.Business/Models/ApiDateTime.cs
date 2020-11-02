using FQCS.Admin.Business.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace FQCS.Admin.Business.Models
{
    public class ApiDateTime
    {
        [JsonProperty("display")]
        public string Display { get; set; }

        private string _iso;
        [JsonProperty("iso")]
        public string Iso
        {
            get
            {
                return _iso;
            }
            set
            {
                _iso = value;
                DateTime utc;
                if (_iso != null && DateTime.TryParse(_iso, out utc))
                    Utc = utc;
            }
        }
        [JsonIgnore]
        public DateTime Utc { get; private set; }
    }
}
