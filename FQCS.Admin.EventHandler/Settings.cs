using System;
using System.Collections.Generic;
using System.Text;

namespace FQCS.Admin.EventHandler
{
    public class Settings
    {
        public string KafkaServer { get; set; }
        public string KafkaUsername { get; set; }
        public string KafkaPassword { get; set; }
        public string GroupId { get; set; }
        public int RetryAfterSecs { get; set; }
    }
}
