using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace FQCS.Admin.Business.Models
{
    public class QCEventMessage
    {
        // this is code from QC device
        [JsonProperty("qc_defect_code")]
        public string QCDefectCode { get; set; }
        [JsonProperty("created_time")]
        public DateTime CreatedTime { get; set; }
        [JsonProperty("identifier")]
        public string Identifier { get; set; }
    }
}
