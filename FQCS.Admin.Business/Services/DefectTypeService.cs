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
using Microsoft.Extensions.DependencyInjection;

namespace FQCS.Admin.Business.Services
{
    public class DefectTypeService : Service
    {
        public DefectTypeService(ServiceInjection inj) : base(inj)
        {
        }

        #region Query DefectType
        public IQueryable<DefectType> DefectTypes
        {
            get
            {
                return context.DefectType;
            }
        }

        public IDictionary<string, object> GetDefectTypeDynamic(
            DefectType row, DefectTypeQueryProjection projection,
            DefectTypeQueryOptions options)
        {
            var obj = new Dictionary<string, object>();
            foreach (var f in projection.GetFieldsArr())
            {
                switch (f)
                {
                    case DefectTypeQueryProjection.INFO:
                        {
                            var entity = row;
                            obj["id"] = entity.Id;
                            obj["name"] = entity.Name;
                            obj["code"] = entity.Code;
                            obj["qc_mapping_code"] = entity.QCMappingCode;
                            obj["description"] = entity.Description;
                            obj["sample_image"] = entity.SampleImage;
                        }
                        break;
                }
            }
            return obj;
        }

        public List<IDictionary<string, object>> GetDefectTypeDynamic(
            IEnumerable<DefectType> rows, DefectTypeQueryProjection projection,
            DefectTypeQueryOptions options)
        {
            var list = new List<IDictionary<string, object>>();
            foreach (var o in rows)
            {
                var obj = GetDefectTypeDynamic(o, projection, options);
                list.Add(obj);
            }
            return list;
        }

        public async Task<QueryResult<IDictionary<string, object>>> QueryDefectTypeDynamic(
            DefectTypeQueryProjection projection,
            DefectTypeQueryOptions options,
            DefectTypeQueryFilter filter = null,
            DefectTypeQuerySort sort = null,
            DefectTypeQueryPaging paging = null)
        {
            var query = DefectTypes;
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
                if (paging != null && (!options.load_all || !DefectTypeQueryOptions.IsLoadAllAllowed))
                    query = query.SelectPage(paging.page, paging.limit);
                #endregion
            }

            if (options.single_only)
            {
                var single = query.SingleOrDefault();
                if (single == null) return null;
                var singleResult = GetDefectTypeDynamic(single, projection, options);
                return new QueryResult<IDictionary<string, object>>()
                {
                    Single = singleResult
                };
            }
            var entities = query.ToList();
            var list = GetDefectTypeDynamic(entities, projection, options);
            var result = new QueryResult<IDictionary<string, object>>();
            result.List = list;
            if (options.count_total) result.Count = totalCount;
            return result;
        }
        #endregion

        #region Create DefectType
        protected void PrepareCreate(DefectType entity)
        {
        }

        public DefectType CreateDefectType(CreateDefectTypeModel model)
        {
            var entity = model.ToDest();
            PrepareCreate(entity);
            return context.DefectType.Add(entity).Entity;
        }
        #endregion

        #region Update DefectType
        public void UpdateDefectType(DefectType entity, UpdateDefectTypeModel model)
        {
            model.CopyTo(entity);
        }

        public (string, string) GetDefectTypeImagePath(DefectType entity,
            string uploadPath, string rootPath)
        {
            var fileService = provider.GetRequiredService<FileService>();
            var folderPath = Path.Combine(rootPath, uploadPath, nameof(DefectType), entity.Id.ToString(), "Sample");
            var result = fileService.GetFilePath(folderPath, rootPath, ext: ".jpg");
            return result;
        }

        public void UpdateDefectTypeImage(DefectType entity, string relPath)
        {
            entity.SampleImage = relPath;
        }

        public async Task SaveReplaceDefectTypeImage(UpdateDefectTypeImageModel model,
            string fullPath, string rootPath, string oldRelPath)
        {
            var fileService = provider.GetRequiredService<FileService>();
            if (oldRelPath != null)
                fileService.DeleteFile(oldRelPath, rootPath);
            await fileService.SaveFile(model.image, fullPath);
        }
        #endregion

        #region Delete DefectType
        public DefectType DeleteDefectType(DefectType entity)
        {
            entity = context.DefectType.Remove(entity).Entity;
            return entity;
        }
        public void DeleteDefectTypeFolder(DefectType entity, string uploadPath, string rootPath)
        {
            var fileService = provider.GetRequiredService<FileService>();
            var folderPath = Path.Combine(uploadPath, nameof(DefectType), entity.Id.ToString());
            fileService.DeleteDirectory(folderPath, rootPath);
        }
        #endregion

        #region Validation
        public ValidationData ValidateGetDefectTypes(
            ClaimsPrincipal principal,
            DefectTypeQueryFilter filter,
            DefectTypeQuerySort sort,
            DefectTypeQueryProjection projection,
            DefectTypeQueryPaging paging,
            DefectTypeQueryOptions options)
        {
            return new ValidationData();
        }

        public ValidationData ValidateCreateDefectType(ClaimsPrincipal principal,
            CreateDefectTypeModel model)
        {
            var validationData = new ValidationData();
            if (string.IsNullOrWhiteSpace(model.Code))
                validationData.Fail("Defect code must not be null", Constants.AppResultCode.FailValidation);
            else if (DefectTypes.Exists(model.Code))
                validationData.Fail("Defect code existed", Constants.AppResultCode.FailValidation);
            if (string.IsNullOrWhiteSpace(model.QCMappingCode))
                validationData.Fail("QC Defect code must not be null", Constants.AppResultCode.FailValidation);
            else if (DefectTypes.ExistsQCMappingCode(model.QCMappingCode))
                validationData.Fail("QC Defect code existed", Constants.AppResultCode.FailValidation);
            if (string.IsNullOrWhiteSpace(model.Name))
                validationData.Fail("Name must not be null", Constants.AppResultCode.FailValidation);
            return validationData;
        }

        public ValidationData ValidateUpdateDefectType(ClaimsPrincipal principal,
            DefectType entity, UpdateDefectTypeModel model)
        {
            var validationData = new ValidationData();
            if (string.IsNullOrWhiteSpace(model.Code))
                validationData.Fail("Defect code must not be null", Constants.AppResultCode.FailValidation);
            else if (DefectTypes.Exists(model.Code))
                validationData.Fail("Defect code existed", Constants.AppResultCode.FailValidation);
            if (string.IsNullOrWhiteSpace(model.QCMappingCode))
                validationData.Fail("QC Defect code must not be null", Constants.AppResultCode.FailValidation);
            else if (DefectTypes.ExistsQCMappingCode(model.QCMappingCode))
                validationData.Fail("QC Defect code existed", Constants.AppResultCode.FailValidation);
            if (string.IsNullOrWhiteSpace(model.Name))
                validationData.Fail("Name must not be null", Constants.AppResultCode.FailValidation);
            return validationData;
        }

        public ValidationData ValidateUpdateDefectTypeImage(ClaimsPrincipal principal,
            DefectType entity, UpdateDefectTypeImageModel model)
        {
            return new ValidationData();
        }

        public ValidationData ValidateDeleteDefectType(ClaimsPrincipal principal,
            DefectType entity)
        {
            return new ValidationData();
        }
        #endregion

    }
}
