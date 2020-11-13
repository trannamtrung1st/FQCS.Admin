using System;
using System.Collections.Generic;
using System.Text;
using static FQCS.Admin.Data.Constants;

namespace FQCS.Admin.Data.Models
{
    public class QCEventDetail
    {
        public string Id { get; set; }
        public int DefectTypeId { get; set; }
        public string EventId { get; set; }

        public virtual DefectType DefectType { get; set; }
        public virtual QCEvent Event { get; set; }
    }
}
