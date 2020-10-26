﻿using System;
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
            public const string ERROR = "error";
        }

        public static class AppDateTimeFormat
        {
            public const string DEFAULT_DATE_FORMAT = "dd/MM/yyyy";
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
