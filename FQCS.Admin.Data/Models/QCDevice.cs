using System;
using System.Collections.Generic;
using System.Text;

namespace FQCS.Admin.Data.Models
{
    public class QCDevice
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Info { get; set; }
        public int ProductionLineId { get; set; }
        public bool Archived { get; set; }
        public DateTime LastUpdated { get; set; }
        public DateTime CreatedTime { get; set; }

        public virtual ProductionLine Line { get; set; }
        public virtual IList<QCEvent> Events { get; set; }
    }
}
