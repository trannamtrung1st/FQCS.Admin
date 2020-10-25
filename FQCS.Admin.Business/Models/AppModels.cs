using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using FQCS.Admin.Data;
using TNT.Core.Helpers.General;

namespace FQCS.Admin.Business.Models
{

    public class AppResult
    {
        [JsonProperty("message")]
        public string Message { get; set; }
        [JsonProperty("data")]
        public object Data { get; set; }
        [JsonProperty("code")]
        public AppResultCode? Code { get; set; }

        public static AppResult Success(object data = null, string mess = null)
        {
            return new AppResult
            {
                Code = AppResultCode.Success,
                Message = mess ?? AppResultCode.Success.DisplayName(),
                Data = data,
            };
        }

        public static AppResult Error(object data = null, string mess = null)
        {
            return new AppResult
            {
                Code = AppResultCode.UnknownError,
                Message = mess ?? AppResultCode.UnknownError.DisplayName(),
                Data = data,
            };
        }

        public static AppResult DependencyDeleteFail(object data = null, string mess = null)
        {
            return new AppResult
            {
                Code = AppResultCode.DependencyDeleteFail,
                Message = mess ?? AppResultCode.DependencyDeleteFail.DisplayName(),
                Data = data,
            };
        }

        public static AppResult FailValidation(object data = null, string mess = null)
        {
            return new AppResult
            {
                Code = AppResultCode.FailValidation,
                Message = mess ?? AppResultCode.FailValidation.DisplayName(),
                Data = data,
            };
        }

        public static AppResult NotFound(object data = null, string mess = null)
        {
            return new AppResult
            {
                Code = AppResultCode.NotFound,
                Message = mess ?? AppResultCode.NotFound.DisplayName(),
                Data = data,
            };
        }

        public static AppResult Unsupported(object data = null, string mess = null)
        {
            return new AppResult
            {
                Code = AppResultCode.Unsupported,
                Message = mess ?? AppResultCode.Unsupported.DisplayName(),
                Data = data,
            };
        }

        public static AppResult Unauthorized(object data = null, string mess = null)
        {
            return new AppResult
            {
                Code = AppResultCode.Unauthorized,
                Message = mess ?? AppResultCode.Unauthorized.DisplayName(),
                Data = data,
            };
        }

        public static AppResult DuplicatedUsername(object data = null, string mess = null)
        {
            return new AppResult
            {
                Code = AppResultCode.DuplicatedUsername,
                Message = mess ?? AppResultCode.DuplicatedUsername.DisplayName(),
                Data = data,
            };
        }

    }

    public class ValidationData
    {
        [JsonProperty("is_valid")]
        public bool IsValid { get; set; }
        [JsonProperty("results")]
        public List<AppResult> Results { get; set; }
        [JsonIgnore]
        public IDictionary<string, object> TempData { get; set; }

        public ValidationData()
        {
            Results = new List<AppResult>();
            IsValid = true;
            TempData = new Dictionary<string, object>();
        }

        public T GetTempData<T>(string key)
        {
            object data = null;
            if (TempData.TryGetValue(key, out data))
                return (T)data;
            return default;
        }

        public ValidationData Fail(string mess = null, AppResultCode? code = null, object data = null)
        {
            Results.Add(new AppResult
            {
                Message = mess ?? code?.DisplayName(),
                Data = data,
                Code = code
            });
            IsValid = false;
            return this;
        }

    }

}
