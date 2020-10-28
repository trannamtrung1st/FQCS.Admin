using System;
using System.Collections.Generic;
using System.Text;

namespace FQCS.Admin.Data.Models
{
    public class DefectType
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string QCMappingCode { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string SampleImage { get; set; }

        public virtual IList<QCEvent> Events { get; set; }
    }
}
