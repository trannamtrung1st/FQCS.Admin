using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace FQCS.Admin.Business.Models
{
    public class QCEventMessage
    {
        [JsonProperty("defect_type_id")]
        public int DefectTypeId { get; set; }
        [JsonProperty("created_time")]
        public DateTime CreatedTime { get; set; }
        [JsonProperty("qc_device_id")]
        public int QCDeviceId { get; set; }
    }
}
