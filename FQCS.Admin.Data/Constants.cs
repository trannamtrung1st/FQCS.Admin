using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FQCS.Admin.Data
{

    public static class Constants
    {
        public static class Data
        {
            public const string CONN_STR = "Server=localhost;Database=FQCS;Trusted_Connection=False;User Id=sa;Password=123456;MultipleActiveResultSets=true";
        }

        public static class RoleName
        {
            public const string ADMIN = "Administrator";
        }

        public enum BatchStatus
        {
            [Display(Name = "New")]
            New = 0,
            [Display(Name = "Started")]
            Started = 1,
            [Display(Name = "Paused")]
            Paused = 2,
            [Display(Name = "Finished")]
            Finished = 3
        }

        public enum AppEventType
        {
            CreateDefectType = 1,
            UpdateDefectType = 2,
            DeleteDefectType = 3,
            UpdateDefectTypeImage = 4,
            CreateProductionLine = 5,
            UpdateProductionLine = 6,
            DeleteProductionLine = 7,
            ChangeProductionLineStatus = 8,
            CreateProductModel = 9,
            UpdateProductModel = 10,
            DeleteProductModel = 11,
            UpdateProductModelImage = 12,
            CreateQCDevice = 13,
            UpdateQCDevice = 14,
            DeleteQCDevice = 15,
            ChangeQCDeviceStatus = 16,
            CreateProductionBatch = 17,
            UpdateProductionBatch = 18,
            DeleteProductionBatch = 19,
            ChangeProductionBatchStatus = 20,
            CreateResource = 1000,
            UpdateResource = 1001,
            DeleteResource = 1002,
        }

    }

}
