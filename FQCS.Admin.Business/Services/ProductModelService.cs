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
using System.IO;
using FQCS.Admin.Business.Helpers;
using Microsoft.Extensions.DependencyInjection;

namespace FQCS.Admin.Business.Services
{
    public class ProductModelService : Service
    {
        public ProductModelService(ServiceInjection inj) : base(inj)
        {
        }

        #region Query ProductModel
        public IQueryable<ProductModel> ProductModels
        {
            get
            {
                return context.ProductModel;
            }
        }

        public IDictionary<string, object> GetProductModelDynamic(
            ProductModel row, ProductModelQueryProjection projection,
            ProductModelQueryOptions options)
        {
            var obj = new Dictionary<string, object>();
            foreach (var f in projection.GetFieldsArr())
            {
                switch (f)
                {
                    case ProductModelQueryProjection.INFO:
                        {
                            var entity = row;
                            obj["id"] = entity.Id;
                            obj["name"] = entity.Name;
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
                            obj["image"] = entity.Image;
                        }
                        break;
                    case ProductModelQueryProjection.SELECT:
                        {
                            var entity = row;
                            obj["id"] = entity.Id;
                            obj["name"] = entity.Name;
                            obj["code"] = entity.Code;
                        }
                        break;
                }
            }
            return obj;
        }

        public List<IDictionary<string, object>> GetProductModelDynamic(
            IEnumerable<ProductModel> rows, ProductModelQueryProjection projection,
            ProductModelQueryOptions options)
        {
            var list = new List<IDictionary<string, object>>();
            foreach (var o in rows)
            {
                var obj = GetProductModelDynamic(o, projection, options);
                list.Add(obj);
            }
            return list;
        }

        public async Task<QueryResult<IDictionary<string, object>>> QueryProductModelDynamic(
            ProductModelQueryProjection projection,
            ProductModelQueryOptions options,
            ProductModelQueryFilter filter = null,
            ProductModelQuerySort sort = null,
            ProductModelQueryPaging paging = null)
        {
            var query = ProductModels;
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
                if (paging != null && (!options.load_all || !ProductModelQueryOptions.IsLoadAllAllowed))
                    query = query.SelectPage(paging.page, paging.limit);
                #endregion
            }

            if (options.single_only)
            {
                var single = query.SingleOrDefault();
                if (single == null) return null;
                var singleResult = GetProductModelDynamic(single, projection, options);
                return new QueryResult<IDictionary<string, object>>()
                {
                    Single = singleResult
                };
            }
            var entities = query.ToList();
            var list = GetProductModelDynamic(entities, projection, options);
            var result = new QueryResult<IDictionary<string, object>>();
            result.List = list;
            if (options.count_total) result.Count = totalCount;
            return result;
        }
        #endregion

        #region Create ProductModel
        protected void PrepareCreate(ProductModel entity)
        {
        }

        public ProductModel CreateProductModel(CreateProductModelModel model)
        {
            var entity = model.ToDest();
            PrepareCreate(entity);
            return context.ProductModel.Add(entity).Entity;
        }
        #endregion

        #region Update ProductModel
        public void UpdateProductModel(ProductModel entity, UpdateProductModelModel model)
        {
            model.CopyTo(entity);
        }

        public (string, string) GetProductModelImagePath(ProductModel entity,
            string uploadPath, string rootPath)
        {
            var fileService = provider.GetRequiredService<FileService>();
            var folderPath = Path.Combine(rootPath, uploadPath, nameof(ProductModel), entity.Id.ToString(), "Image");
            var result = fileService.GetFilePath(folderPath, rootPath, ext: ".jpg");
            return result;
        }

        public void UpdateProductModelImage(ProductModel entity, string relPath)
        {
            entity.Image = relPath;
        }

        public async Task SaveReplaceProductModelImage(UpdateProductModelImageModel model, 
            string fullPath, string rootPath, string oldRelPath)
        {
            var fileService = provider.GetRequiredService<FileService>();
            if (oldRelPath != null)
                fileService.DeleteFile(oldRelPath, rootPath);
            await fileService.SaveFile(model.image, fullPath);
        }
        #endregion

        #region Delete ProductModel
        public ProductModel DeleteProductModel(ProductModel entity)
        {
            entity = context.ProductModel.Remove(entity).Entity;
            return entity;
        }
        public void DeleteProductModelFolder(ProductModel entity, string uploadPath, string rootPath)
        {
            var fileService = provider.GetRequiredService<FileService>();
            var folderPath = Path.Combine(uploadPath, nameof(ProductModel), entity.Id.ToString());
            fileService.DeleteDirectory(folderPath, rootPath);
        }
        #endregion

        #region Validation
        public ValidationData ValidateGetProductModels(
            ClaimsPrincipal principal,
            ProductModelQueryFilter filter,
            ProductModelQuerySort sort,
            ProductModelQueryProjection projection,
            ProductModelQueryPaging paging,
            ProductModelQueryOptions options)
        {
            return new ValidationData();
        }

        public ValidationData ValidateCreateProductModel(ClaimsPrincipal principal,
            CreateProductModelModel model)
        {
            return new ValidationData();
        }

        public ValidationData ValidateUpdateProductModel(ClaimsPrincipal principal,
            ProductModel entity, UpdateProductModelModel model)
        {
            return new ValidationData();
        }

        public ValidationData ValidateUpdateProductModelImage(ClaimsPrincipal principal,
            ProductModel entity, UpdateProductModelImageModel model)
        {
            return new ValidationData();
        }

        public ValidationData ValidateDeleteProductModel(ClaimsPrincipal principal,
            ProductModel entity)
        {
            return new ValidationData();
        }
        #endregion

    }
}
