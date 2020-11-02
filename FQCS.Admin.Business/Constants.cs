using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using FQCS.Admin.Business.Helpers;

namespace FQCS.Admin.Business
{
    public static class Constants
    {
        public static class DeviceConstants
        {
            public const string AppClientScheme = "AppClient";
        }

        public static class QCEventOps
        {
            public const string GET_EVENTS = "get_events";
            public const string DOWNLOAD_IMAGES = "download_images";
            public const string UPDATE_STATUS = "update_status";
            public const string TRIGGER_SEND = "trigger_send";
            public const string CLEAR_ALL = "clear_all";
        }

        public static class ContentType
        {
            public const string SPREADSHEET = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        }

        public enum AppResultCode
        {
            [Display(Name = "Unknown error")]
            UnknownError = 1,
            [Display(Name = "Success")]
            Success = 2,
            [Display(Name = "Fail validation")]
            FailValidation = 3,
            [Display(Name = "Not found")]
            NotFound = 4,
            [Display(Name = "Unsupported")]
            Unsupported = 5,
            [Display(Name = "Can not delete because of dependencies")]
            DependencyDeleteFail = 6,
            [Display(Name = "Unauthorized")]
            Unauthorized = 7,
            [Display(Name = "Username has already existed")]
            DuplicatedUsername = 8

        }

        public class JWT
        {
            public const string ISSUER = "fqcs1st";
            public const string AUDIENCE = "fqcs1st";
            public const string SECRET_KEY = "ASDFOIPJJP812340-89ADSFPOUADSFH809-3152-798OHASDFHPOU1324-8ASDF";

            public const string REFRESH_ISSUER = "refresh_fqcs1st";
            public const string REFRESH_AUDIENCE = "refresh_fqcs1st";
            public const string REFRESH_SECRET_KEY = "FSPDIU2093T-ASDGPIOSDGPHASDG-EWRQWGWQEGWE-QWER-QWER13412-AQRQWR";
        }

        public static class AppClaimType
        {
            public const string UserName = "username";
            public const string FullName = "full_name";
        }

        public static class AppOAuthScope
        {
            public const string ROLES = "roles";
        }

        public static class ApiEndpoint
        {
            public const string ROLE_API = "api/roles";
            public const string USER_API = "api/users";
            public const string RESOURCE_API = "api/resources";
            public const string APP_EVENT_API = "api/app-events";
            public const string DEFECT_TYPE_API = "api/defect-types";
            public const string PRODUCTION_LINE_API = "api/production-lines";
            public const string PRODUCT_MODEL_API = "api/product-models";
            public const string QC_DEVICE_API = "api/qc-devices";
            public const string PRODUCTION_BATCH_API = "api/production-batches";
            public const string QC_EVENT_API = "api/qc-events";
            public const string REPORT_API = "api/reports";
            public const string APP_CONFIG_API = "api/app-configs";
            public const string ERROR = "error";
        }

        public static class AppDateTimeFormat
        {
            public const string DEFAULT_DATE_FORMAT = "dd/MM/yyyy";
            public const string APP_CLIENT_AUTH_FORMAT = "ddMMyyyyHHmmss";
        }

        public static class AppTimeZone
        {
            public static readonly TimeZoneInfo Default = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
        }

        public enum BoolOptions
        {
            T = 1, F = 2, B = 3 //true, false, both
        }
    }

}
