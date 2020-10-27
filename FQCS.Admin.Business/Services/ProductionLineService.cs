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
    public class ProductionLineService : Service
    {
        public ProductionLineService(ServiceInjection inj) : base(inj)
        {
        }

        #region Query ProductionLine
        public IQueryable<ProductionLine> ProductionLines
        {
            get
            {
                return context.ProductionLine;
            }
        }

        public IDictionary<string, object> GetProductionLineDynamic(
            ProductionLine row, ProductionLineQueryProjection projection,
            ProductionLineQueryOptions options)
        {
            var obj = new Dictionary<string, object>();
            foreach (var f in projection.GetFieldsArr())
            {
                switch (f)
                {
                    case ProductionLineQueryProjection.INFO:
                        {
                            var entity = row;
                            obj["id"] = entity.Id;
                            obj["code"] = entity.Code;
                            obj["info"] = entity.Info;
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
                            obj["disabled"] = entity.Disabled;
                        }
                        break;
                }
            }
            return obj;
        }

        public List<IDictionary<string, object>> GetProductionLineDynamic(
            IEnumerable<ProductionLine> rows, ProductionLineQueryProjection projection,
            ProductionLineQueryOptions options)
        {
            var list = new List<IDictionary<string, object>>();
            foreach (var o in rows)
            {
                var obj = GetProductionLineDynamic(o, projection, options);
                list.Add(obj);
            }
            return list;
        }

        public async Task<QueryResult<IDictionary<string, object>>> QueryProductionLineDynamic(
            ProductionLineQueryProjection projection,
            ProductionLineQueryOptions options,
            ProductionLineQueryFilter filter = null,
            ProductionLineQuerySort sort = null,
            ProductionLineQueryPaging paging = null)
        {
            var query = ProductionLines;
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
                if (paging != null && (!options.load_all || !ProductionLineQueryOptions.IsLoadAllAllowed))
                    query = query.SelectPage(paging.page, paging.limit);
                #endregion
            }

            if (options.single_only)
            {
                var single = query.SingleOrDefault();
                if (single == null) return null;
                var singleResult = GetProductionLineDynamic(single, projection, options);
                return new QueryResult<IDictionary<string, object>>()
                {
                    Single = singleResult
                };
            }
            var entities = query.ToList();
            var list = GetProductionLineDynamic(entities, projection, options);
            var result = new QueryResult<IDictionary<string, object>>();
            result.List = list;
            if (options.count_total) result.Count = totalCount;
            return result;
        }
        #endregion

        #region Create ProductionLine
        protected void PrepareCreate(ProductionLine entity)
        {
            entity.CreatedTime = DateTime.UtcNow;
            entity.LastUpdated = entity.CreatedTime;
        }

        public ProductionLine CreateProductionLine(CreateProductionLineModel model)
        {
            var entity = model.ToDest();
            PrepareCreate(entity);
            return context.ProductionLine.Add(entity).Entity;
        }
        #endregion

        #region Update ProductionLine
        protected void PrepareUpdate(ProductionLine entity)
        {
            entity.LastUpdated = DateTime.UtcNow;
        }

        public void UpdateProductionLine(ProductionLine entity, UpdateProductionLineModel model)
        {
            model.CopyTo(entity);
            PrepareUpdate(entity);
        }

        public void ChangeProductionLineStatus(ProductionLine entity, ChangeProductionLineStatusModel model)
        {
            entity.Disabled = model.Disabled;
            PrepareUpdate(entity);
        }
        #endregion

        #region Delete ProductionLine
        public ProductionLine DeleteProductionLine(ProductionLine entity)
        {
            return context.ProductionLine.Remove(entity).Entity;
        }
        #endregion

        #region Validation
        public ValidationData ValidateGetProductionLines(
            ClaimsPrincipal principal,
            ProductionLineQueryFilter filter,
            ProductionLineQuerySort sort,
            ProductionLineQueryProjection projection,
            ProductionLineQueryPaging paging,
            ProductionLineQueryOptions options)
        {
            return new ValidationData();
        }

        public ValidationData ValidateCreateProductionLine(ClaimsPrincipal principal,
            CreateProductionLineModel model)
        {
            return new ValidationData();
        }

        public ValidationData ValidateUpdateProductionLine(ClaimsPrincipal principal,
            ProductionLine entity, UpdateProductionLineModel model)
        {
            return new ValidationData();
        }

        public ValidationData ValidateChangeProductionLineStatus(ClaimsPrincipal principal,
            ProductionLine entity, ChangeProductionLineStatusModel model)
        {
            return new ValidationData();
        }

        public ValidationData ValidateDeleteProductionLine(ClaimsPrincipal principal,
            ProductionLine entity)
        {
            return new ValidationData();
        }
        #endregion

    }
}
