using Newtonsoft.Json;
using FQCS.Admin.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FQCS.Admin.Business.Models
{
    public class CreateQCDeviceModel : MappingModel<QCDevice>
    {
        public CreateQCDeviceModel()
        {
        }

        public CreateQCDeviceModel(QCDevice src) : base(src)
        {
        }

        [JsonProperty("code")]
        public string Code { get; set; }
        [JsonProperty("info")]
        public string Info { get; set; }
        [JsonProperty("production_line_id")]
        public int? ProductionLineId { get; set; }
        [JsonProperty("app_config_id")]
        public string AppConfigId { get; set; }
        [JsonProperty("device_api_base_url")]
        public string DeviceAPIBaseUrl { get; set; }
    }

    public class UpdateQCDeviceModel : MappingModel<QCDevice>
    {
        public UpdateQCDeviceModel()
        {
        }

        public UpdateQCDeviceModel(QCDevice src) : base(src)
        {
        }

        [JsonProperty("code")]
        public string Code { get; set; }
        [JsonProperty("info")]
        public string Info { get; set; }
        [JsonProperty("production_line_id")]
        public int? ProductionLineId { get; set; }
        [JsonProperty("app_config_id")]
        public string AppConfigId { get; set; }
        [JsonProperty("device_api_base_url")]
        public string DeviceAPIBaseUrl { get; set; }
    }

    public class ChangeQCDeviceStatusModel
    {
        [JsonProperty("archived")]
        public bool Archived { get; set; }
    }

    #region Query
    public class QCDeviceQueryProjection
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
        public const string P_LINE = "pline";
        public const string CFG = "cfg";
        public const string SELECT = "select";

        public static readonly IDictionary<string, string[]> MAPS = new Dictionary<string, string[]>
        {
            {P_LINE, new []{ $"{nameof(QCDevice.Line)}" }}
        };
    }


    public class QCDeviceQuerySort
    {
        public const string CODE = "code";
        private const string DEFAULT = "a" + CODE;
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

    public class QCDeviceQueryFilter
    {
        public int? id { get; set; }
        public string code { get; set; }
    }

    public class QCDeviceQueryPaging
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

    public class QCDeviceQueryOptions
    {
        public bool count_total { get; set; }
        public string date_format { get; set; }
        public bool single_only { get; set; }
        public bool load_all { get; set; }

        public const bool IsLoadAllAllowed = true;
    }
    #endregion
}
