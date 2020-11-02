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

namespace FQCS.Admin.Business.Services
{
    public class AppConfigService : Service
    {
        public AppConfigService(ServiceInjection inj) : base(inj)
        {
        }

        #region Query AppConfig
        public IQueryable<AppConfig> AppConfigs
        {
            get
            {
                return context.AppConfig;
            }
        }

        public IDictionary<string, object> GetAppConfigDynamic(
            AppConfig row, AppConfigQueryProjection projection,
            AppConfigQueryOptions options)
        {
            var obj = new Dictionary<string, object>();
            foreach (var f in projection.GetFieldsArr())
            {
                switch (f)
                {
                    case AppConfigQueryProjection.INFO:
                        {
                            var entity = row;
                            obj["id"] = entity.Id;
                            obj["name"] = entity.Name;
                            obj["client_id"] = entity.ClientId;
                            obj["is_default"] = entity.IsDefault;
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
                        }
                        break;
                    case AppConfigQueryProjection.SELECT:
                        {
                            var entity = row;
                            obj["id"] = entity.Id;
                            obj["name"] = entity.Name;
                            obj["is_default"] = entity.IsDefault;
                        }
                        break;
                }
            }
            return obj;
        }

        public List<IDictionary<string, object>> GetAppConfigDynamic(
            IEnumerable<AppConfig> rows, AppConfigQueryProjection projection,
            AppConfigQueryOptions options)
        {
            var list = new List<IDictionary<string, object>>();
            foreach (var o in rows)
            {
                var obj = GetAppConfigDynamic(o, projection, options);
                list.Add(obj);
            }
            return list;
        }

        public async Task<QueryResult<IDictionary<string, object>>> QueryAppConfigDynamic(
            AppConfigQueryProjection projection,
            AppConfigQueryOptions options,
            AppConfigQueryFilter filter = null,
            AppConfigQuerySort sort = null,
            AppConfigQueryPaging paging = null)
        {
            var query = AppConfigs;
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
                if (paging != null && (!options.load_all || !AppConfigQueryOptions.IsLoadAllAllowed))
                    query = query.SelectPage(paging.page, paging.limit);
                #endregion
            }

            if (options.single_only)
            {
                var single = query.SingleOrDefault();
                if (single == null) return null;
                var singleResult = GetAppConfigDynamic(single, projection, options);
                return new QueryResult<IDictionary<string, object>>()
                {
                    Single = singleResult
                };
            }
            var entities = query.ToList();
            var list = GetAppConfigDynamic(entities, projection, options);
            var result = new QueryResult<IDictionary<string, object>>();
            result.List = list;
            if (options.count_total) result.Count = totalCount;
            return result;
        }
        #endregion

        #region Create AppConfig
        protected void PrepareCreate(AppConfig entity)
        {
            entity.CreatedTime = DateTime.UtcNow;
            entity.LastUpdated = entity.CreatedTime;
        }

        public AppConfig CreateAppConfig(CreateAppConfigModel model)
        {
            var entity = model.ToDest();
            PrepareCreate(entity);
            return context.AppConfig.Add(entity).Entity;
        }
        #endregion

        #region Update AppConfig
        public AppConfig ChangeDefaultConfig(AppConfig entity, AppConfig oldDefault)
        {
            entity.IsDefault = true;
            PrepareUpdate(entity);
            if (oldDefault != null)
            {
                oldDefault.IsDefault = false;
                PrepareUpdate(oldDefault);
            }
            return entity;
        }

        public void PrepareUpdate(AppConfig entity)
        {
            entity.LastUpdated = DateTime.UtcNow;
        }

        public void UpdateAppConfig(AppConfig entity, UpdateAppConfigModel model)
        {
            model.CopyTo(entity);
            if (!string.IsNullOrWhiteSpace(model.ClientSecretReset))
                entity.ClientSecret = model.ClientSecretReset;
            PrepareUpdate(entity);
        }
        #endregion

        #region Delete AppConfig
        public AppConfig DeleteAppConfig(AppConfig entity)
        {
            return context.AppConfig.Remove(entity).Entity;
        }
        #endregion

        #region Validation
        public ValidationData ValidateGetAppConfigs(
            ClaimsPrincipal principal,
            AppConfigQueryFilter filter,
            AppConfigQuerySort sort,
            AppConfigQueryProjection projection,
            AppConfigQueryPaging paging,
            AppConfigQueryOptions options)
        {
            return new ValidationData();
        }

        public ValidationData ValidateCreateAppConfig(ClaimsPrincipal principal,
            CreateAppConfigModel model)
        {
            return new ValidationData();
        }

        public ValidationData ValidateUpdateAppConfig(ClaimsPrincipal principal,
            AppConfig entity, UpdateAppConfigModel model)
        {
            return new ValidationData();
        }

        public ValidationData ValidateChangeDefaultConfig(ClaimsPrincipal principal,
            AppConfig entity, ChangeDefaultConfigModel model)
        {
            return new ValidationData();
        }


        public ValidationData ValidateDeleteAppConfig(ClaimsPrincipal principal,
            AppConfig entity)
        {
            return new ValidationData();
        }
        #endregion

    }
}
