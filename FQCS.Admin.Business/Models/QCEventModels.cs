using FQCS.Admin.Data.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FQCS.Admin.Business.Models
{
    public class QCEventMessage
    {
        // this is code from QC device
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("qc_defect_code")]
        public string QCDefectCode { get; set; }
        [JsonProperty("created_time")]
        public DateTime CreatedTime { get; set; }
        [JsonProperty("identifier")]
        public string Identifier { get; set; }
        [JsonProperty("left_b64_image")]
        public string LeftB64Image { get; set; }
        [JsonProperty("right_b64_image")]
        public string RightB64Image { get; set; }
    }


    #region Query
    public class QCEventQueryProjection
    {
        private const string DEFAULT = INFO;
        private string _fields = DEFAULT;
        public string fields
        {
            get
            {
                return _fields;
            }
            set
            {
                if (value?.Length > 0)
                {
                    _fields = value;
                    _fieldsArr = value.Split(',').OrderBy(v => v).ToArray();
                }
            }
        }

        private string[] _fieldsArr = DEFAULT.Split(',');
        public string[] GetFieldsArr()
        {
            return _fieldsArr;
        }

        //---------------------------------------

        public const string INFO = "info";
        public const string BATCH = "batch";
        public const string D_TYPE = "dtype";
        public const string IMAGE = "image";
        public const string SELECT = "select";

        public static readonly IDictionary<string, string[]> MAPS = new Dictionary<string, string[]>
        {
            {BATCH, new []{ $"{nameof(QCEvent.Batch)}" }},
            {D_TYPE, new []{ $"{nameof(QCEvent.DefectType)}" }},
        };
    }

    public class QCEventQuerySort
    {
        public const string TIME = "time";
        private const string DEFAULT = "d" + TIME;
        private string _sorts = DEFAULT;
        public string sorts
        {
            get
            {
                return _sorts;
            }
            set
            {
                if (value?.Length > 0)
                {
                    _sorts = value;
                    _sortsArr = value.Split(',');
                }
            }
        }

        public string[] _sortsArr = DEFAULT.Split(',');
        public string[] GetSortsArr()
        {
            return _sortsArr;
        }

    }

    public class QCEventQueryFilter
    {
        public string id { get; set; }
        public int? batch_id { get; set; }
    }

    public class QCEventQueryPaging
    {
        private int _page = 1;
        public int page
        {
            get
            {
                return _page;
            }
            set
            {
                _page = value > 0 ? value : _page;
            }
        }

        private int _limit = 10;
        public int limit
        {
            get
            {
                return _limit;
            }
            set
            {
                if (value >= 1 && value <= 100)
                    _limit = value;
            }
        }
    }

    public class QCEventQueryOptions
    {
        public bool count_total { get; set; }
        public string date_format { get; set; }
        public bool single_only { get; set; }
        public bool load_all { get; set; }

        public const bool IsLoadAllAllowed = false;
    }
    #endregion
}
