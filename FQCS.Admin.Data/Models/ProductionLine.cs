using System;
using System.Collections.Generic;
using System.Text;

namespace FQCS.Admin.Data.Models
{
    public class ProductionLine
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Info { get; set; }
        public DateTime LastUpdated { get; set; }
        public DateTime CreatedTime { get; set; }
        public bool Disabled { get; set; }

        public virtual IList<ProductionBatch> Batches { get; set; }
        public virtual IList<QCDevice> Devices { get; set; }
    }
}
