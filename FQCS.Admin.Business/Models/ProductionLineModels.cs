﻿using Newtonsoft.Json;
using FQCS.Admin.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FQCS.Admin.Business.Models
{
    public class CreateProductionLineModel : MappingModel<ProductionLine>
    {
        public CreateProductionLineModel()
        {
        }

        public CreateProductionLineModel(ProductionLine src) : base(src)
        {
        }

        [JsonProperty("code")]
        public string Code { get; set; }
        [JsonProperty("info")]
        public string Info { get; set; }
    }

    public class UpdateProductionLineModel : MappingModel<ProductionLine>
    {
        public UpdateProductionLineModel()
        {
        }

        public UpdateProductionLineModel(ProductionLine src) : base(src)
        {
        }


        [JsonProperty("code")]
        public string Code { get; set; }
        [JsonProperty("info")]
        public string Info { get; set; }

    }

    public class ChangeProductionLineStatusModel
    {
        [JsonProperty("disabled")]
        public bool Disabled { get; set; }
    }

    #region Query
    public class ProductionLineQueryProjection
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
        public const string SELECT = "select";

        public static readonly IDictionary<string, string[]> MAPS = new Dictionary<string, string[]>
        {
        };
    }


    public class ProductionLineQuerySort
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

    public class ProductionLineQueryFilter
    {
        public int? id { get; set; }
        public string code { get; set; }
    }

    public class ProductionLineQueryPaging
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

    public class ProductionLineQueryOptions
    {
        public bool count_total { get; set; }
        public string date_format { get; set; }
        public bool single_only { get; set; }
        public bool load_all { get; set; }

        public const bool IsLoadAllAllowed = true;
    }
    #endregion
}
