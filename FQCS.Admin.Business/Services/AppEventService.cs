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
using static FQCS.Admin.Business.Constants;

namespace FQCS.Admin.Business.Services
{
    public class AppEventService : Service
    {
        public AppEventService(ServiceInjection inj) : base(inj)
        {
        }

        #region Query AppEvent
        public IQueryable<AppEvent> AppEvents
        {
            get
            {
                return context.AppEvent;
            }
        }

        public IDictionary<string, object> GetAppEventDynamic(
            AppEvent row, AppEventQueryProjection projection,
            AppEventQueryOptions options)
        {
            var obj = new Dictionary<string, object>();
            foreach (var f in projection.GetFieldsArr())
            {
                switch (f)
                {
                    case AppEventQueryProjection.INFO:
                        {
                            var entity = row;
                            obj["id"] = entity.Id;
                            var time = entity.CreatedTime
                                .ToDefaultTimeZone();
                            var timeStr = time.ToString(options.date_format);
                            obj["created_time"] = new
                            {
                                display = timeStr,
                                iso = $"{time.ToUniversalTime():s}Z"
                            };
                            obj["description"] = entity.Description;
                            obj["type"] = entity.Type;
                            obj["user_id"] = entity.UserId;
                        }
                        break;
                    case AppEventQueryProjection.USER:
                        {
                            var entity = row.User;
                            obj["user"] = new
                            {
                                id = entity.Id,
                                username = entity.UserName,
                                full_name = entity.FullName,
                            };
                        }
                        break;
                }
            }
            return obj;
        }

        public List<IDictionary<string, object>> GetAppEventDynamic(
            IEnumerable<AppEvent> rows, AppEventQueryProjection projection,
            AppEventQueryOptions options)
        {
            var list = new List<IDictionary<string, object>>();
            foreach (var o in rows)
            {
                var obj = GetAppEventDynamic(o, projection, options);
                list.Add(obj);
            }
            return list;
        }

        public async Task<QueryResult<IDictionary<string, object>>> QueryAppEventDynamic(
            AppEventQueryProjection projection,
            AppEventQueryOptions options,
            AppEventQueryFilter filter = null,
            AppEventQuerySort sort = null,
            AppEventQueryPaging paging = null)
        {
            var query = AppEvents;
            #region General
            if (filter != null) query = query.Filter(filter);
            int? totalCount = null;
            if (options.count_total) totalCount = query.Count();
            #endregion
            if (!options.single_only)
            {
                #region List query
                if (sort != null) query = query.Sort(sort);
                if (paging != null && (!options.load_all || !AppEventQueryOptions.IsLoadAllAllowed))
                    query = query.SelectPage(paging.page, paging.limit);
                #endregion
            }

            if (options.single_only)
            {
                var single = query.SingleOrDefault();
                if (single == null) return null;
                var singleResult = GetAppEventDynamic(single, projection, options);
                return new QueryResult<IDictionary<string, object>>()
                {
                    Single = singleResult
                };
            }
            var entities = query.ToList();
            var list = GetAppEventDynamic(entities, projection, options);
            var result = new QueryResult<IDictionary<string, object>>();
            result.List = list;
            if (options.count_total) result.Count = totalCount;
            return result;
        }
        #endregion

        protected void PrepareCreate(AppEvent entity)
        {
            entity.CreatedTime = DateTime.UtcNow;
        }

        #region Resource
        public AppEvent CreateResource(Resource entity, ClaimsPrincipal principal)
        {
            string username = principal.FindFirstValue(AppClaimType.UserName);
            var ev = new AppEvent
            {
                Data = null,
                Description = $"User {username} created a resource named {entity.Name}, id: {entity.Id}",
                Type = Data.Constants.AppEventType.CreateResource,
                UserId = principal.Identity.Name
            };
            PrepareCreate(ev);
            return context.AppEvent.Add(ev).Entity;
        }

        public AppEvent UpdateResource(Resource entity, ClaimsPrincipal principal)
        {
            string username = principal.FindFirstValue(AppClaimType.UserName);
            var ev = new AppEvent
            {
                Data = null,
                Description = $"User {username} updated resource id: {entity.Id}",
                Type = Data.Constants.AppEventType.UpdateResource,
                UserId = principal.Identity.Name
            };
            PrepareCreate(ev);
            return context.AppEvent.Add(ev).Entity;
        }

        public AppEvent DeleteResource(Resource entity, ClaimsPrincipal principal)
        {
            string username = principal.FindFirstValue(AppClaimType.UserName);
            var ev = new AppEvent
            {
                Data = null,
                Description = $"User {username} deleted resource id: {entity.Id}",
                Type = Data.Constants.AppEventType.DeleteResource,
                UserId = principal.Identity.Name
            };
            PrepareCreate(ev);
            return context.AppEvent.Add(ev).Entity;
        }
        #endregion

        #region Defect Type
        public AppEvent CreateDefectType(DefectType entity, ClaimsPrincipal principal)
        {
            string username = principal.FindFirstValue(AppClaimType.UserName);
            var ev = new AppEvent
            {
                Data = null,
                Description = $"User {username} created a defect type named {entity.Name}, id: {entity.Id}",
                Type = Data.Constants.AppEventType.CreateDefectType,
                UserId = principal.Identity.Name
            };
            PrepareCreate(ev);
            return context.AppEvent.Add(ev).Entity;
        }

        public AppEvent UpdateDefectTypeImage(DefectType entity, ClaimsPrincipal principal)
        {
            string username = principal.FindFirstValue(AppClaimType.UserName);
            var ev = new AppEvent
            {
                Data = null,
                Description = $"User {username} updated defect type's image - id: {entity.Id}",
                Type = Data.Constants.AppEventType.UpdateDefectType,
                UserId = principal.Identity.Name
            };
            PrepareCreate(ev);
            return context.AppEvent.Add(ev).Entity;
        }

        public AppEvent UpdateDefectType(DefectType entity, ClaimsPrincipal principal)
        {
            string username = principal.FindFirstValue(AppClaimType.UserName);
            var ev = new AppEvent
            {
                Data = null,
                Description = $"User {username} updated defect type id: {entity.Id}",
                Type = Data.Constants.AppEventType.UpdateDefectType,
                UserId = principal.Identity.Name
            };
            PrepareCreate(ev);
            return context.AppEvent.Add(ev).Entity;
        }

        public AppEvent DeleteDefectType(DefectType entity, ClaimsPrincipal principal)
        {
            string username = principal.FindFirstValue(AppClaimType.UserName);
            var ev = new AppEvent
            {
                Data = null,
                Description = $"User {username} deleted defect type id: {entity.Id}",
                Type = Data.Constants.AppEventType.DeleteDefectType,
                UserId = principal.Identity.Name
            };
            PrepareCreate(ev);
            return context.AppEvent.Add(ev).Entity;
        }
        #endregion

        #region Production Line
        public AppEvent CreateProductionLine(ProductionLine entity, ClaimsPrincipal principal)
        {
            string username = principal.FindFirstValue(AppClaimType.UserName);
            var ev = new AppEvent
            {
                Data = null,
                Description = $"User {username} created a production line with code {entity.Code}, id: {entity.Id}",
                Type = Data.Constants.AppEventType.CreateProductionLine,
                UserId = principal.Identity.Name
            };
            PrepareCreate(ev);
            return context.AppEvent.Add(ev).Entity;
        }

        public AppEvent ChangeProductionLineStatus(ProductionLine entity, ClaimsPrincipal principal)
        {
            string username = principal.FindFirstValue(AppClaimType.UserName);
            var ev = new AppEvent
            {
                Data = null,
                Description = $"User {username} changed disabled status: {entity.Disabled} - id: {entity.Id}",
                Type = Data.Constants.AppEventType.ChangeProductionLineStatus,
                UserId = principal.Identity.Name
            };
            PrepareCreate(ev);
            return context.AppEvent.Add(ev).Entity;
        }

        public AppEvent UpdateProductionLine(ProductionLine entity, ClaimsPrincipal principal)
        {
            string username = principal.FindFirstValue(AppClaimType.UserName);
            var ev = new AppEvent
            {
                Data = null,
                Description = $"User {username} updated production line - id: {entity.Id}",
                Type = Data.Constants.AppEventType.UpdateProductionLine,
                UserId = principal.Identity.Name
            };
            PrepareCreate(ev);
            return context.AppEvent.Add(ev).Entity;
        }

        public AppEvent DeleteProductionLine(ProductionLine entity, ClaimsPrincipal principal)
        {
            string username = principal.FindFirstValue(AppClaimType.UserName);
            var ev = new AppEvent
            {
                Data = null,
                Description = $"User {username} deleted production line id: {entity.Id}",
                Type = Data.Constants.AppEventType.DeleteProductionLine,
                UserId = principal.Identity.Name
            };
            PrepareCreate(ev);
            return context.AppEvent.Add(ev).Entity;
        }
        #endregion

        #region Product Model
        public AppEvent CreateProductModel(ProductModel entity, ClaimsPrincipal principal)
        {
            string username = principal.FindFirstValue(AppClaimType.UserName);
            var ev = new AppEvent
            {
                Data = null,
                Description = $"User {username} created a product model named {entity.Name}, id: {entity.Id}",
                Type = Data.Constants.AppEventType.CreateProductModel,
                UserId = principal.Identity.Name
            };
            PrepareCreate(ev);
            return context.AppEvent.Add(ev).Entity;
        }

        public AppEvent UpdateProductModelImage(ProductModel entity, ClaimsPrincipal principal)
        {
            string username = principal.FindFirstValue(AppClaimType.UserName);
            var ev = new AppEvent
            {
                Data = null,
                Description = $"User {username} updated product model's image - id: {entity.Id}",
                Type = Data.Constants.AppEventType.UpdateProductModel,
                UserId = principal.Identity.Name
            };
            PrepareCreate(ev);
            return context.AppEvent.Add(ev).Entity;
        }

        public AppEvent UpdateProductModel(ProductModel entity, ClaimsPrincipal principal)
        {
            string username = principal.FindFirstValue(AppClaimType.UserName);
            var ev = new AppEvent
            {
                Data = null,
                Description = $"User {username} updated product model id: {entity.Id}",
                Type = Data.Constants.AppEventType.UpdateProductModel,
                UserId = principal.Identity.Name
            };
            PrepareCreate(ev);
            return context.AppEvent.Add(ev).Entity;
        }

        public AppEvent DeleteProductModel(ProductModel entity, ClaimsPrincipal principal)
        {
            string username = principal.FindFirstValue(AppClaimType.UserName);
            var ev = new AppEvent
            {
                Data = null,
                Description = $"User {username} deleted product model id: {entity.Id}",
                Type = Data.Constants.AppEventType.DeleteProductModel,
                UserId = principal.Identity.Name
            };
            PrepareCreate(ev);
            return context.AppEvent.Add(ev).Entity;
        }
        #endregion

        #region QC Device
        public AppEvent CreateQCDevice(QCDevice entity, ClaimsPrincipal principal)
        {
            string username = principal.FindFirstValue(AppClaimType.UserName);
            var ev = new AppEvent
            {
                Data = null,
                Description = $"User {username} created a QC device with code {entity.Code}, id: {entity.Id}",
                Type = Data.Constants.AppEventType.CreateQCDevice,
                UserId = principal.Identity.Name
            };
            PrepareCreate(ev);
            return context.AppEvent.Add(ev).Entity;
        }

        public AppEvent ChangeQCDeviceStatus(QCDevice entity, ClaimsPrincipal principal)
        {
            string username = principal.FindFirstValue(AppClaimType.UserName);
            var ev = new AppEvent
            {
                Data = null,
                Description = $"User {username} changed archived status: {entity.Archived} - id: {entity.Id}",
                Type = Data.Constants.AppEventType.ChangeQCDeviceStatus,
                UserId = principal.Identity.Name
            };
            PrepareCreate(ev);
            return context.AppEvent.Add(ev).Entity;
        }

        public AppEvent UpdateQCDevice(QCDevice entity, ClaimsPrincipal principal)
        {
            string username = principal.FindFirstValue(AppClaimType.UserName);
            var ev = new AppEvent
            {
                Data = null,
                Description = $"User {username} updated QC device - id: {entity.Id}",
                Type = Data.Constants.AppEventType.UpdateQCDevice,
                UserId = principal.Identity.Name
            };
            PrepareCreate(ev);
            return context.AppEvent.Add(ev).Entity;
        }

        public AppEvent DeleteQCDevice(QCDevice entity, ClaimsPrincipal principal)
        {
            string username = principal.FindFirstValue(AppClaimType.UserName);
            var ev = new AppEvent
            {
                Data = null,
                Description = $"User {username} deleted QC device id: {entity.Id}",
                Type = Data.Constants.AppEventType.DeleteQCDevice,
                UserId = principal.Identity.Name
            };
            PrepareCreate(ev);
            return context.AppEvent.Add(ev).Entity;
        }
        #endregion

        #region Validation
        public ValidationData ValidateGetAppEvents(
            ClaimsPrincipal principal,
            AppEventQueryFilter filter,
            AppEventQuerySort sort,
            AppEventQueryProjection projection,
            AppEventQueryPaging paging,
            AppEventQueryOptions options)
        {
            return new ValidationData();
        }
        #endregion

    }
}
