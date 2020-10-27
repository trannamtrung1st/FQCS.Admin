using System;
using System.Collections.Generic;
using System.Text;

namespace FQCS.Admin.Data.Models
{
    public class ProductModel
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Info { get; set; }
        public string Image { get; set; }
        public DateTime LastUpdated { get; set; }
        public DateTime CreatedTime { get; set; }

        public virtual IList<ProductionBatch> Batches { get; set; }

    }
}
