using System;
using System.Collections.Generic;
using System.Text;
using static FQCS.Admin.Data.Constants;

namespace FQCS.Admin.Data.Models
{
    public class AppEvent
    {
        public int Id { get; set; }
        public DateTime CreatedTime { get; set; }
        public AppEventType Type { get; set; }
        public string Description { get; set; }
        public string UserId { get; set; }
        public string Data { get; set; }

        public virtual AppUser User { get; set; }
    }
}
