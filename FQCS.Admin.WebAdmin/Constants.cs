﻿using System;
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
            public const string ADMIN_ONLY = "admin_only";
        }
    }

}
