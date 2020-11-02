using Microsoft.EntityFrameworkCore;
using FQCS.Admin.Business.Models;
using FQCS.Admin.Business.Queries;
using FQCS.Admin.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TNT.Core.Helpers.DI;
using FQCS.Admin.Business.Helpers;
using System.Net.Http;
using System.Net.Http.Headers;
using TNT.Core.Http;

namespace FQCS.Admin.Business.Services
{
    public class QCDeviceService : Service
    {
        [Inject]
        protected readonly IdentityService identityService;

        public QCDeviceService(ServiceInjection inj) : base(inj)
        {
        }

        #region Query QCDevice
        public IQueryable<QCDevice> QCDevices
        {
            get
            {
                return context.QCDevice;
            }
        }

        public IDictionary<string, object> GetQCDeviceDynamic(
            QCDevice row, QCDeviceQueryProjection projection,
            QCDeviceQueryOptions options)
        {
            var obj = new Dictionary<string, object>();
            foreach (var f in projection.GetFieldsArr())
            {
                switch (f)
                {
                    case QCDeviceQueryProjection.INFO:
                        {
                            var entity = row;
                            obj["id"] = entity.Id;
                            obj["code"] = entity.Code;
                            obj["info"] = entity.Info;
                            obj["production_line_id"] = entity.ProductionLineId;
                            obj["app_config_id"] = entity.AppConfigId;
                            obj["device_api_base_url"] = entity.DeviceAPIBaseUrl;
                            var time = entity.CreatedTime
                                .ToDefaultTimeZone();
                            var timeStr = time.ToString(options.date_format);
                            obj["created_time"] = new
                            {
                                display = timeStr,
                                iso = $"{time.ToUniversalTime():s}Z"
                            };
                            time = entity.LastUpdated
                                .ToDefaultTimeZone();
                            timeStr = time.ToString(options.date_format);
                            obj["last_updated"] = new
                            {
                                display = timeStr,
                                iso = $"{time.ToUniversalTime():s}Z"
                            };
                            obj["archived"] = entity.Archived;
                        }
                        break;
                    case QCDeviceQueryProjection.P_LINE:
                        {
                            var entity = row.Line;
                            if (entity != null)
                                obj["production_line"] = new
                                {
                                    id = entity.Id,
                                    code = entity.Code,
                                    disabled = entity.Disabled
                                };
                        }
                        break;
                    case QCDeviceQueryProjection.CFG:
                        {
                            var entity = row.Config;
                            if (entity != null)
                                obj["app_config"] = new
                                {
                                    id = entity.Id,
                                    name = entity.Name,
                                    client_id = entity.ClientId,
                                };
                        }
                        break;
                }
            }
            return obj;
        }

        public List<IDictionary<string, object>> GetQCDeviceDynamic(
            IEnumerable<QCDevice> rows, QCDeviceQueryProjection projection,
            QCDeviceQueryOptions options)
        {
            var list = new List<IDictionary<string, object>>();
            foreach (var o in rows)
            {
                var obj = GetQCDeviceDynamic(o, projection, options);
                list.Add(obj);
            }
            return list;
        }

        public async Task<QueryResult<IDictionary<string, object>>> QueryQCDeviceDynamic(
            QCDeviceQueryProjection projection,
            QCDeviceQueryOptions options,
            QCDeviceQueryFilter filter = null,
            QCDeviceQuerySort sort = null,
            QCDeviceQueryPaging paging = null)
        {
            var query = QCDevices;
            #region General
            if (filter != null) query = query.Filter(filter);
            query = query.Project(projection);
            int? totalCount = null;
            if (options.count_total) totalCount = query.Count();
            #endregion
            if (!options.single_only)
            {
                #region List query
                if (sort != null) query = query.Sort(sort);
                if (paging != null && (!options.load_all || !QCDeviceQueryOptions.IsLoadAllAllowed))
                    query = query.SelectPage(paging.page, paging.limit);
                #endregion
            }

            if (options.single_only)
            {
                var single = query.SingleOrDefault();
                if (single == null) return null;
                var singleResult = GetQCDeviceDynamic(single, projection, options);
                return new QueryResult<IDictionary<string, object>>()
                {
                    Single = singleResult
                };
            }
            var entities = query.ToList();
            var list = GetQCDeviceDynamic(entities, projection, options);
            var result = new QueryResult<IDictionary<string, object>>();
            result.List = list;
            if (options.count_total) result.Count = totalCount;
            return result;
        }
        #endregion

        #region Create QCDevice
        protected void PrepareCreate(QCDevice entity)
        {
            entity.CreatedTime = DateTime.UtcNow;
            entity.LastUpdated = entity.CreatedTime;
        }

        public QCDevice CreateQCDevice(CreateQCDeviceModel model)
        {
            var entity = model.ToDest();
            PrepareCreate(entity);
            return context.QCDevice.Add(entity).Entity;
        }
        #endregion

        #region Update QCDevice
        protected void PrepareUpdate(QCDevice entity)
        {
            entity.LastUpdated = DateTime.UtcNow;
        }

        public void UpdateQCDevice(QCDevice entity, UpdateQCDeviceModel model)
        {
            model.CopyTo(entity);
            PrepareUpdate(entity);
        }

        public void ChangeQCDeviceStatus(QCDevice entity, ChangeQCDeviceStatusModel model)
        {
            entity.Archived = model.Archived;
            PrepareUpdate(entity);
        }
        #endregion

        #region Delete QCDevice
        public QCDevice DeleteQCDevice(QCDevice entity)
        {
            return context.QCDevice.Remove(entity).Entity;
        }
        #endregion

        #region Validation
        public ValidationData ValidateGetQCDevices(
            ClaimsPrincipal principal,
            QCDeviceQueryFilter filter,
            QCDeviceQuerySort sort,
            QCDeviceQueryProjection projection,
            QCDeviceQueryPaging paging,
            QCDeviceQueryOptions options)
        {
            return new ValidationData();
        }

        public ValidationData ValidateSendCommandToDeviceAPI(ClaimsPrincipal principal,
            SendCommandToDeviceAPIModel model)
        {
            return new ValidationData();
        }

        public ValidationData ValidateCreateQCDevice(ClaimsPrincipal principal,
            CreateQCDeviceModel model)
        {
            return new ValidationData();
        }

        public ValidationData ValidateUpdateQCDevice(ClaimsPrincipal principal,
            QCDevice entity, UpdateQCDeviceModel model)
        {
            return new ValidationData();
        }

        public ValidationData ValidateChangeQCDeviceStatus(ClaimsPrincipal principal,
            QCDevice entity, ChangeQCDeviceStatusModel model)
        {
            return new ValidationData();
        }

        public ValidationData ValidateDeleteQCDevice(ClaimsPrincipal principal,
            QCDevice entity)
        {
            return new ValidationData();
        }
        #endregion

        #region Send command
        public async Task<int> SendCommandGetEvents(SendCommandToDeviceAPIModel model,
            QCDevice entity, AppConfig deviceConfig)
        {
            var queryStr = new Dictionary<string, object>()
            {
                { "load_all", true },
                { "sent", false }
            }.ToQueryString();
            var url = $"{entity.DeviceAPIBaseUrl}/api/qc-events?{queryStr}";
            var request = new HttpRequestMessage(HttpMethod.Get, new Uri(url, UriKind.Absolute));
            var authHeader = identityService.GetAppClientAuthHeader(deviceConfig);
            request.Headers.Authorization = new AuthenticationHeaderValue(
                Constants.DeviceConstants.AppClientScheme, authHeader);
            var resp = await Global.HttpDevice.SendAsync(request);
            if (!resp.IsSuccessStatusCode)
                throw new Exception("Error sending command");
            var respObj = resp.Content.ReadAsAsync<List<QCEventDeviceModel>>();
            return 0;
        }
        #endregion

    }
}
