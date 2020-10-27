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
    public class ProductionBatchService : Service
    {
        public ProductionBatchService(ServiceInjection inj) : base(inj)
        {
        }

        #region Query ProductionBatch
        public IQueryable<ProductionBatch> ProductionBatchs
        {
            get
            {
                return context.ProductionBatch;
            }
        }

        public IDictionary<string, object> GetProductionBatchDynamic(
            ProductionBatch row, ProductionBatchQueryProjection projection,
            ProductionBatchQueryOptions options)
        {
            var obj = new Dictionary<string, object>();
            foreach (var f in projection.GetFieldsArr())
            {
                switch (f)
                {
                    case ProductionBatchQueryProjection.INFO:
                        {
                            var entity = row;
                            obj["id"] = entity.Id;
                            obj["code"] = entity.Code;
                            obj["info"] = entity.Info;
                            obj["total_amount"] = entity.TotalAmount;
                            obj["production_line_id"] = entity.ProductionLineId;
                            obj["product_model_id"] = entity.ProductModelId;
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
                            obj["status"] = entity.Status;
                        }
                        break;
                    case ProductionBatchQueryProjection.P_LINE:
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
                    case ProductionBatchQueryProjection.P_MODEL:
                        {
                            var entity = row.Model;
                            if (entity != null)
                                obj["product_model"] = new
                                {
                                    id = entity.Id,
                                    code = entity.Code,
                                };
                        }
                        break;
                    case ProductionBatchQueryProjection.SELECT:
                        {
                            var entity = row;
                            obj["id"] = entity.Id;
                            obj["code"] = entity.Code;
                        }
                        break;
                }
            }
            return obj;
        }

        public List<IDictionary<string, object>> GetProductionBatchDynamic(
            IEnumerable<ProductionBatch> rows, ProductionBatchQueryProjection projection,
            ProductionBatchQueryOptions options)
        {
            var list = new List<IDictionary<string, object>>();
            foreach (var o in rows)
            {
                var obj = GetProductionBatchDynamic(o, projection, options);
                list.Add(obj);
            }
            return list;
        }

        public async Task<QueryResult<IDictionary<string, object>>> QueryProductionBatchDynamic(
            ProductionBatchQueryProjection projection,
            ProductionBatchQueryOptions options,
            ProductionBatchQueryFilter filter = null,
            ProductionBatchQuerySort sort = null,
            ProductionBatchQueryPaging paging = null)
        {
            var query = ProductionBatchs;
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
                if (paging != null && (!options.load_all || !ProductionBatchQueryOptions.IsLoadAllAllowed))
                    query = query.SelectPage(paging.page, paging.limit);
                #endregion
            }

            if (options.single_only)
            {
                var single = query.SingleOrDefault();
                if (single == null) return null;
                var singleResult = GetProductionBatchDynamic(single, projection, options);
                return new QueryResult<IDictionary<string, object>>()
                {
                    Single = singleResult
                };
            }
            var entities = query.ToList();
            var list = GetProductionBatchDynamic(entities, projection, options);
            var result = new QueryResult<IDictionary<string, object>>();
            result.List = list;
            if (options.count_total) result.Count = totalCount;
            return result;
        }
        #endregion

        #region Create ProductionBatch
        protected void PrepareCreate(ProductionBatch entity)
        {
            entity.CreatedTime = DateTime.UtcNow;
            entity.LastUpdated = entity.CreatedTime;
        }

        public ProductionBatch CreateProductionBatch(CreateProductionBatchModel model)
        {
            var entity = model.ToDest();
            PrepareCreate(entity);
            return context.ProductionBatch.Add(entity).Entity;
        }
        #endregion

        #region Update ProductionBatch
        protected void PrepareUpdate(ProductionBatch entity)
        {
            entity.LastUpdated = DateTime.UtcNow;
        }

        public void UpdateProductionBatch(ProductionBatch entity, UpdateProductionBatchModel model)
        {
            model.CopyTo(entity);
            PrepareUpdate(entity);
        }

        public void ChangeProductionBatchStatus(ProductionBatch entity, ChangeProductionBatchStatusModel model)
        {
            if (model.Status == Data.Constants.BatchStatus.New)
                throw new Exception("Invalid status change");
            switch (model.Status)
            {
                case Data.Constants.BatchStatus.Started:
                    entity.StartedTime = DateTime.UtcNow;
                    break;
                case Data.Constants.BatchStatus.Finished:
                    entity.FinishedTime = DateTime.UtcNow;
                    break;
            }
            entity.Status = model.Status;
            PrepareUpdate(entity);
        }
        #endregion

        #region Delete ProductionBatch
        public ProductionBatch DeleteProductionBatch(ProductionBatch entity)
        {
            return context.ProductionBatch.Remove(entity).Entity;
        }
        #endregion

        #region Validation
        public ValidationData ValidateGetProductionBatchs(
            ClaimsPrincipal principal,
            ProductionBatchQueryFilter filter,
            ProductionBatchQuerySort sort,
            ProductionBatchQueryProjection projection,
            ProductionBatchQueryPaging paging,
            ProductionBatchQueryOptions options)
        {
            return new ValidationData();
        }

        public ValidationData ValidateCreateProductionBatch(ClaimsPrincipal principal,
            CreateProductionBatchModel model)
        {
            return new ValidationData();
        }

        public ValidationData ValidateUpdateProductionBatch(ClaimsPrincipal principal,
            ProductionBatch entity, UpdateProductionBatchModel model)
        {
            return new ValidationData();
        }

        public ValidationData ValidateChangeProductionBatchStatus(ClaimsPrincipal principal,
            ProductionBatch entity, ChangeProductionBatchStatusModel model)
        {
            return new ValidationData();
        }

        public ValidationData ValidateDeleteProductionBatch(ClaimsPrincipal principal,
            ProductionBatch entity)
        {
            return new ValidationData();
        }
        #endregion

    }
}
