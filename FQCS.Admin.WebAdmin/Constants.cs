using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FQCS.Admin.WebAdmin
{
    public static class Constants
    {

        public static class Routing
        {
            public const string DASHBOARD = "/dashboard";
            public const string LOGIN = "/identity/login";
            public const string LOGOUT = "/identity/logout";
            public const string REGISTER = "/identity/register";
            public const string IDENTITY = "/identity";
            public const string RESOURCE = "/resource";
            public const string RESOURCE_CREATE = "/resource/create";
            public const string RESOURCE_DETAIL = "/resource/{id}";
            public const string DEFECT_TYPE = "/defecttype";
            public const string DEFECT_TYPE_CREATE = "/defecttype/create";
            public const string DEFECT_TYPE_DETAIL = "/defecttype/{id}";
            public const string PRODUCTION_LINE = "/productionline";
            public const string PRODUCTION_LINE_CREATE = "/productionline/create";
            public const string PRODUCTION_LINE_DETAIL = "/productionline/{id}";
            public const string PRODUCT_MODEL = "/productmodel";
            public const string PRODUCT_MODEL_CREATE = "/productmodel/create";
            public const string PRODUCT_MODEL_DETAIL = "/productmodel/{id}";
            public const string QC_DEVICE = "/qcdevice";
            public const string QC_DEVICE_CREATE = "/qcdevice/create";
            public const string QC_DEVICE_DETAIL = "/qcdevice/{id}";
            public const string PRODUCTION_BATCH = "/productionbatch";
            public const string PRODUCTION_BATCH_CREATE = "/productionbatch/create";
            public const string PRODUCTION_BATCH_DETAIL = "/productionbatch/{id}";
            public const string QC_EVENT = "/qcevent";
            public const string QC_EVENT_DETAIL = "/qcevent/{id}";
            public const string REPORT = "/report";
            public const string ADMIN_ONLY = "/adminonly";
            public const string ACCESS_DENIED = "/accessdenied";
            public const string STATUS = "/status";
            public const string ERROR = "/error";
            public const string ERROR_CONTROLLER = "error";
            public const string INDEX = "/";
        }

        public static class AppController
        {
        }

        public static class AppCookie
        {
            public const string TOKEN = "_appuat";
        }

        public static class AppView
        {
            public const string MESSAGE = "MessageView";
            public const string STATUS = "StatusView";
            public const string ERROR = "ErrorView";
        }

        public static class Menu
        {
            public const string DASHBOARD = "dashboard";
            public const string RESOURCE = "resource";
            public const string DEFECT_TYPE = "defect_type";
            public const string PRODUCTION_LINE = "production_line";
            public const string PRODUCT_MODEL = "product_model";
            public const string QC_DEVICE = "qc_device";
            public const string QC_EVENT = "qc_event";
            public const string PRODUCTION_BATCH = "production_batch";
            public const string REPORT = "report";
            public const string ADMIN_ONLY = "admin_only";
        }
    }

}
