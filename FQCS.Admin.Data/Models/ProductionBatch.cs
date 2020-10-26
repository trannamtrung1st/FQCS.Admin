using System;
using System.Collections.Generic;
using System.Text;
using static FQCS.Admin.Data.Constants;

namespace FQCS.Admin.Data.Models
{
    public class ProductionBatch
    {
        public int Id { get; set; }
        public int ProductionLineId { get; set; }
        public int ProductModelId { get; set; }
        public int TotalAmount { get; set; }
        public BatchStatus Status { get; set; }
        public DateTime LastUpdated { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime? StartedTime { get; set; }
        public DateTime? FinishedTime { get; set; }

        public virtual ProductionLine Line { get; set; }
        public virtual ProductModel Model { get; set; }
        public virtual IList<QCEvent> Events { get; set; }
    }
}
