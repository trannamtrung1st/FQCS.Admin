using System;
using System.Collections.Generic;
using System.Text;
using static FQCS.Admin.Data.Constants;

namespace FQCS.Admin.Data.Models
{
    public class QCEvent
    {
        public QCEvent()
        {
            Details = new List<QCEventDetail>();
        }

        public string Id { get; set; }
        public string Description { get; set; }
        public DateTime LastUpdated { get; set; }
        public DateTime CreatedTime { get; set; }
        public int ProductionBatchId { get; set; }
        public int QCDeviceId { get; set; }
        public string LeftImage { get; set; }
        public string RightImage { get; set; }
        public string SideImages { get; set; }

        public virtual ProductionBatch Batch { get; set; }
        public virtual QCDevice Device { get; set; }
        public virtual IList<QCEventDetail> Details { get; set; }

    }
}
