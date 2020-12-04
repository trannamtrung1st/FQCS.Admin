using System;
using System.Collections.Generic;
using System.Text;

namespace FQCS.Admin.ConsoleClient
{
    public class Settings
    {
        public string StartZooCmd { get; set; }
        public string StopZooCmd { get; set; }
        public string StopKafkaCmd { get; set; }
        public string StartKafkaCmd { get; set; }
        public string ResetKafkaCmd { get; set; }
    }
}
