﻿using Microsoft.EntityFrameworkCore;
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
using System.IO;
using ClosedXML.Excel;
using static FQCS.Admin.Business.Constants;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using EFCore.BulkExtensions;

namespace FQCS.Admin.Business.Services
{
    public interface IQCEventService
    {
        IQueryable<QCEventDetail> QCEventDetails { get; }
        IQueryable<QCEvent> QCEvents { get; }

        QCEvent ConvertToQCEvent(QCEventDeviceModel model, QCDevice device);
        QCEvent ConvertToQCEvent(QCEventMessage model, QCDevice device);
        QCEvent CreateQCEvent(QCEvent entity);
        List<IDictionary<string, object>> GetQCEventDynamic(IEnumerable<QCEvent> rows, QCEventQueryProjection projection, QCEventQueryOptions options, string qcFolderPath, IFileService fileService);
        IDictionary<string, object> GetQCEventDynamic(QCEvent row, QCEventQueryProjection projection, QCEventQueryOptions options, string qcFolderPath, IFileService fileService);
        IQueryable<QCEvent> GetQueryableQCEventForUpdate(QCEventQueryOptions options, QCEventQueryFilter filter = null, QCEventQuerySort sort = null, QCEventQueryPaging paging = null);
        Task<QueryResult<QCEvent>> QueryQCEvent(QCEventQueryProjection projection, QCEventQueryOptions options, QCEventQueryFilter filter = null, QCEventQuerySort sort = null, QCEventQueryPaging paging = null);
        Task<QueryResult<IDictionary<string, object>>> QueryQCEventDynamic(QCEventQueryProjection projection, QCEventQueryOptions options, string qcFolderPath, QCEventQueryFilter filter = null, QCEventQuerySort sort = null, QCEventQueryPaging paging = null);
        int UpdateEventsSeenStatus(IQueryable<QCEvent> entities, bool val);
        ValidationData ValidateGetQCEvents(ClaimsPrincipal principal, QCEventQueryFilter filter, QCEventQuerySort sort, QCEventQueryProjection projection, QCEventQueryPaging paging, QCEventQueryOptions options);
        ValidationData ValidateQCMessage(QCEventMessage model);
        ValidationData ValidateUpdateSeenStatus(ClaimsPrincipal principal, QCEventQueryFilter filter, QCEventQuerySort sort, QCEventQueryPaging paging, QCEventQueryOptions options);
    }

    public class QCEventService : Service, IQCEventService
    {
        public QCEventService(ServiceInjection inj) : base(inj)
        {
        }

        #region Query QCEvent
        public IQueryable<QCEvent> QCEvents
        {
            get
            {
                return context.QCEvent;
            }
        }
        public IQueryable<QCEventDetail> QCEventDetails
        {
            get
            {
                return context.QCEventDetail;
            }
        }

        public IDictionary<string, object> GetQCEventDynamic(
            QCEvent row, QCEventQueryProjection projection,
            QCEventQueryOptions options, string qcFolderPath, IFileService fileService)
        {
            var obj = new Dictionary<string, object>();
            foreach (var f in projection.GetFieldsArr())
            {
                switch (f)
                {
                    case QCEventQueryProjection.INFO:
                        {
                            var entity = row;
                            obj["id"] = entity.Id;
                            var details = entity.Details.Select(o => new
                            {
                                id = o.Id,
                                defect_type_id = o.DefectTypeId,
                                defect_type = new
                                {
                                    id = o.DefectType.Id,
                                    code = o.DefectType.Code,
                                    name = o.DefectType.Name
                                }
                            }).ToList();
                            obj["details"] = details;
                            obj["description"] = entity.Description;
                            obj["production_batch_id"] = entity.ProductionBatchId;
                            obj["qc_device_id"] = entity.QCDeviceId;
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
                            obj["seen"] = entity.Seen;
                        }
                        break;
                    case QCEventQueryProjection.BATCH:
                        {
                            var entity = row.Batch;
                            if (entity != null)
                                obj["batch"] = new
                                {
                                    id = entity.Id,
                                    code = entity.Code,
                                };
                        }
                        break;
                    case QCEventQueryProjection.IMAGE:
                        {
                            if (!options.single_only)
                                throw new Exception("Only single option can query image field");
                            var entity = row;
                            if (entity.LeftImage != null)
                            {
                                var finalPath = Path.Combine(qcFolderPath, entity.LeftImage);
                                if (File.Exists(finalPath))
                                {
                                    var img = File.ReadAllBytes(finalPath);
                                    var img64 = Convert.ToBase64String(img);
                                    obj["left_image"] = img64;
                                }
                            }
                            if (entity.RightImage != null)
                            {
                                var finalPath = Path.Combine(qcFolderPath, entity.RightImage);
                                if (File.Exists(finalPath))
                                {
                                    var img = File.ReadAllBytes(finalPath);
                                    var img64 = Convert.ToBase64String(img);
                                    obj["right_image"] = img64;
                                }
                            }
                            if (entity.SideImages != null)
                            {
                                var sideImages = JsonConvert.DeserializeObject<IEnumerable<string>>(entity.SideImages);
                                var sideImagesB64 = new List<string>();
                                obj["side_images"] = sideImagesB64;
                                foreach (var iPath in sideImages)
                                {
                                    var fullPath = fileService.GetFilePath(qcFolderPath, null, iPath).Item2;
                                    if (File.Exists(fullPath))
                                    {
                                        var img = File.ReadAllBytes(fullPath);
                                        var img64 = Convert.ToBase64String(img);
                                        sideImagesB64.Add(img64);
                                    }
                                }
                            }
                        }
                        break;
                }
            }
            return obj;
        }

        public List<IDictionary<string, object>> GetQCEventDynamic(
            IEnumerable<QCEvent> rows, QCEventQueryProjection projection,
            QCEventQueryOptions options, string qcFolderPath, IFileService fileService)
        {
            var list = new List<IDictionary<string, object>>();
            foreach (var o in rows)
            {
                var obj = GetQCEventDynamic(o, projection, options, qcFolderPath, fileService);
                list.Add(obj);
            }
            return list;
        }

        public async Task<QueryResult<IDictionary<string, object>>> QueryQCEventDynamic(
            QCEventQueryProjection projection,
            QCEventQueryOptions options, string qcFolderPath,
            QCEventQueryFilter filter = null,
            QCEventQuerySort sort = null,
            QCEventQueryPaging paging = null)
        {
            var fileService = provider.GetRequiredService<IFileService>();
            var query = QCEvents;
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
                if (paging != null && (!options.load_all || !QCEventQueryOptions.IsLoadAllAllowed))
                    query = query.SelectPage(paging.page, paging.limit);
                #endregion
            }

            if (options.single_only)
            {
                var single = query.SingleOrDefault();
                if (single == null) return null;
                var singleResult = GetQCEventDynamic(single, projection, options, qcFolderPath, fileService);
                return new QueryResult<IDictionary<string, object>>()
                {
                    Single = singleResult
                };
            }
            var entities = query.ToList();
            var list = GetQCEventDynamic(entities, projection, options, qcFolderPath, fileService);
            var result = new QueryResult<IDictionary<string, object>>();
            result.List = list;
            if (options.count_total) result.Count = totalCount;
            return result;
        }

        public async Task<QueryResult<QCEvent>> QueryQCEvent(
            QCEventQueryProjection projection,
            QCEventQueryOptions options,
            QCEventQueryFilter filter = null,
            QCEventQuerySort sort = null,
            QCEventQueryPaging paging = null)
        {
            var query = QCEvents;
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
                if (paging != null && (!options.load_all || !QCEventQueryOptions.IsLoadAllAllowed))
                    query = query.SelectPage(paging.page, paging.limit);
                #endregion
            }

            if (options.single_only)
            {
                var single = query.SingleOrDefault();
                if (single == null) return null;
                return new QueryResult<QCEvent>()
                {
                    Single = single
                };
            }
            var result = new QueryResult<QCEvent>();
            result.List = query.ToList();
            if (options.count_total) result.Count = totalCount;
            return result;
        }

        public IQueryable<QCEvent> GetQueryableQCEventForUpdate(
            QCEventQueryOptions options,
            QCEventQueryFilter filter = null,
            QCEventQuerySort sort = null,
            QCEventQueryPaging paging = null)
        {
            var query = QCEvents;
            if (filter != null) query = query.Filter(filter);
            if (!options.single_only)
            {
                if (paging != null && (!options.load_all || !QCEventQueryOptions.IsLoadAllAllowed))
                {
                    if (sort != null) query = query.Sort(sort);
                    query = query.SelectPage(paging.page, paging.limit);
                }
            }
            return query;
        }

        #endregion

        #region Create QCEventService
        protected void PrepareCreate(QCEvent entity)
        {
            entity.LastUpdated = entity.CreatedTime;
        }

        public QCEvent CreateQCEvent(QCEvent entity)
        {
            PrepareCreate(entity);
            return context.QCEvent.Add(entity).Entity;
        }
        #endregion

        #region Update
        protected void PrepareUpdate(QCEvent entity)
        {
            entity.LastUpdated = DateTime.UtcNow;
        }

        public int UpdateEventsSeenStatus(IQueryable<QCEvent> entities, bool val)
        {
            var updated = new QCEvent
            {
                Seen = val
            };
            PrepareUpdate(updated);
            return entities.BatchUpdate(updated, new List<string> {
                nameof(QCEvent.Seen), nameof(QCEvent.LastUpdated) });
        }
        #endregion

        public QCEvent ConvertToQCEvent(QCEventMessage model, QCDevice device)
        {
            var defectTypeService = provider.GetRequiredService<IDefectTypeService>();
            var proBatchService = provider.GetRequiredService<IProductionBatchService>();

            var proBatch = proBatchService.ProductionBatchs.InLine(device.ProductionLineId.Value)
                .RunningAtTime(model.CreatedTime).Select(o => new ProductionBatch
                {
                    Id = o.Id,
                    ProductModelId = o.ProductModelId
                }).First();
            var defCodes = model.Details.Select(o => o.QCDefectCode).ToList();
            var defectTypesMap = defectTypeService.DefectTypes.QCMappingCodes(defCodes)
                .Select(o => new
                {
                    o.Id,
                    o.QCMappingCode
                }).ToDictionary(o => o.QCMappingCode);
            var entity = new QCEvent
            {
                Id = model.Id,
                CreatedTime = model.CreatedTime,
                QCDeviceId = device.Id,
                ProductionBatchId = proBatch.Id,
                Details = model.Details.Select(o => new QCEventDetail
                {
                    DefectTypeId = defectTypesMap[o.QCDefectCode].Id,
                    Id = o.Id,
                }).ToList()
            };
            return entity;
        }

        public QCEvent ConvertToQCEvent(QCEventDeviceModel model, QCDevice device)
        {
            var defectTypeService = provider.GetRequiredService<IDefectTypeService>();
            var proBatchService = provider.GetRequiredService<IProductionBatchService>();

            var proBatch = proBatchService.ProductionBatchs.InLine(device.ProductionLineId.Value)
                .RunningAtTime(model.CreatedTime.Utc.Value).Select(o => new ProductionBatch
                {
                    Id = o.Id,
                    ProductModelId = o.ProductModelId
                }).First();
            var defCodes = model.Details.Select(o => o.DefectTypeCode).ToList();
            var defectTypesMap = defectTypeService.DefectTypes.QCMappingCodes(defCodes)
                .Select(o => new
                {
                    o.Id,
                    o.QCMappingCode
                }).ToDictionary(o => o.QCMappingCode);
            var entity = new QCEvent
            {
                Id = model.Id,
                CreatedTime = model.CreatedTime.Utc.Value,
                QCDeviceId = device.Id,
                ProductionBatchId = proBatch.Id,
                Details = model.Details.Select(o => new QCEventDetail
                {
                    DefectTypeId = defectTypesMap[o.DefectTypeCode].Id,
                    Id = o.Id,
                }).ToList(),
                LeftImage = model.LeftImage,
                RightImage = model.RightImage,
                SideImages = model.SideImages == null ? null :
                    JsonConvert.SerializeObject(model.SideImages)
            };
            return entity;
        }

        #region Validation
        public ValidationData ValidateQCMessage(
            QCEventMessage model)
        {
            var validationData = new ValidationData();
            var existed = QCEvents.Exists(model.Id);
            if (existed)
                return validationData.Fail(mess: "Existed ID", AppResultCode.FailValidation);
            if (string.IsNullOrWhiteSpace(model.Identifier))
                return validationData.Fail(mess: "Identifier must not be null", AppResultCode.FailValidation);
            return validationData;
        }

        public ValidationData ValidateUpdateSeenStatus(
            ClaimsPrincipal principal,
            QCEventQueryFilter filter,
            QCEventQuerySort sort,
            QCEventQueryPaging paging,
            QCEventQueryOptions options)
        {
            return new ValidationData();
        }

        public ValidationData ValidateGetQCEvents(
            ClaimsPrincipal principal,
            QCEventQueryFilter filter,
            QCEventQuerySort sort,
            QCEventQueryProjection projection,
            QCEventQueryPaging paging,
            QCEventQueryOptions options)
        {
            return new ValidationData();
        }
        #endregion

    }
}
