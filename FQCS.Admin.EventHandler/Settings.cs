using System;
using System.Collections.Generic;
using System.Text;

namespace FQCS.Admin.EventHandler
{
    public class Settings
    {
        public string KafkaServer;
        public string KafkaUsername;
        public string KafkaPassword;
        public string GroupId;
        public int RetryAfterSecs;
    }
}
