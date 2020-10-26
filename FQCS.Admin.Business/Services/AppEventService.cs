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

        #region Create AppEvent
        protected void PrepareCreate(AppEvent entity)
        {
            entity.CreatedTime = DateTime.UtcNow;
        }

        public AppEvent CreateResource(Resource resource, ClaimsPrincipal principal)
        {
            string username = principal.FindFirstValue(AppClaimType.UserName);
            var entity = new AppEvent
            {
                Data = null,
                Description = $"User {username} created a resource named {resource.Name}, id: {resource.Id}",
                Type = Data.Constants.AppEventType.CreateResource,
                UserId = principal.Identity.Name
            };
            PrepareCreate(entity);
            return context.AppEvent.Add(entity).Entity;
        }

        public AppEvent UpdateResource(Resource resource, ClaimsPrincipal principal)
        {
            string username = principal.FindFirstValue(AppClaimType.UserName);
            var entity = new AppEvent
            {
                Data = null,
                Description = $"User {username} updated resource id: {resource.Id}",
                Type = Data.Constants.AppEventType.UpdateResource,
                UserId = principal.Identity.Name
            };
            PrepareCreate(entity);
            return context.AppEvent.Add(entity).Entity;
        }

        public AppEvent DeleteResource(Resource resource, ClaimsPrincipal principal)
        {
            string username = principal.FindFirstValue(AppClaimType.UserName);
            var entity = new AppEvent
            {
                Data = null,
                Description = $"User {username} deleted resource id: {resource.Id}",
                Type = Data.Constants.AppEventType.DeleteResource,
                UserId = principal.Identity.Name
            };
            PrepareCreate(entity);
            return context.AppEvent.Add(entity).Entity;
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
