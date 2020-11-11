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
using System.IO;
using ClosedXML.Excel;
using static FQCS.Admin.Business.Constants;
using Microsoft.Extensions.DependencyInjection;

namespace FQCS.Admin.Business.Services
{
    public class QCEventService : Service
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

        public IDictionary<string, object> GetQCEventDynamic(
            QCEvent row, QCEventQueryProjection projection,
            QCEventQueryOptions options, string qcFolderPath)
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
                            obj["defect_type_id"] = entity.DefectTypeId;
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
                    case QCEventQueryProjection.D_TYPE:
                        {
                            var entity = row.DefectType;
                            if (entity != null)
                                obj["defect_type"] = new
                                {
                                    id = entity.Id,
                                    code = entity.Code,
                                    name = entity.Name
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
                        }
                        break;
                }
            }
            return obj;
        }

        public List<IDictionary<string, object>> GetQCEventDynamic(
            IEnumerable<QCEvent> rows, QCEventQueryProjection projection,
            QCEventQueryOptions options, string qcFolderPath)
        {
            var list = new List<IDictionary<string, object>>();
            foreach (var o in rows)
            {
                var obj = GetQCEventDynamic(o, projection, options, qcFolderPath);
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
                var singleResult = GetQCEventDynamic(single, projection, options, qcFolderPath);
                return new QueryResult<IDictionary<string, object>>()
                {
                    Single = singleResult
                };
            }
            var entities = query.ToList();
            var list = GetQCEventDynamic(entities, projection, options, qcFolderPath);
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
        public QCEvent ConvertToQCEvent(QCEventMessage model, QCDevice device,
            DefectType defectType)
        {
            var proBatchService = provider.GetRequiredService<ProductionBatchService>();

            var proBatch = proBatchService.ProductionBatchs.InLine(device.ProductionLineId.Value)
                .RunningAtTime(model.CreatedTime).Select(o => new ProductionBatch
                {
                    Id = o.Id,
                    ProductModelId = o.ProductModelId
                }).First();

            var entity = new QCEvent
            {
                Id = model.Id,
                CreatedTime = model.CreatedTime,
                QCDeviceId = device.Id,
                DefectTypeId = defectType?.Id,
                Description = defectType != null ?
                    $"Defect type at batch: {proBatch.Id}-{defectType.Name}-{defectType.Code} at {model.CreatedTime}"
                    : $"Pass at batch: {proBatch.Id} at {model.CreatedTime}",
                ProductionBatchId = proBatch.Id,
            };
            return entity;
        }

        public QCEvent ConvertToQCEvent(QCEventDeviceModel model, QCDevice device)
        {
            var defectTypeService = provider.GetRequiredService<DefectTypeService>();
            var proBatchService = provider.GetRequiredService<ProductionBatchService>();

            var defectType = model.DefectTypeCode != null ?
                defectTypeService.DefectTypes.QCMappingCode(model.DefectTypeCode)
                .Select(o => new DefectType
                {
                    Id = o.Id,
                    Code = o.Code,
                    Name = o.Name
                }).First() : null;
            var proBatch = proBatchService.ProductionBatchs.InLine(device.ProductionLineId.Value)
                .RunningAtTime(model.CreatedTime.Utc.Value).Select(o => new ProductionBatch
                {
                    Id = o.Id,
                    ProductModelId = o.ProductModelId
                }).First();

            var entity = new QCEvent
            {
                Id = model.Id,
                CreatedTime = model.CreatedTime.Utc.Value,
                QCDeviceId = device.Id,
                DefectTypeId = defectType?.Id,
                Description = defectType != null ?
                    $"Defect type at batch: {proBatch.Id}-{defectType.Name}-{defectType.Code} at {model.CreatedTime.Utc}"
                    : $"Pass at batch: {proBatch.Id} at {model.CreatedTime.Utc}",
                ProductionBatchId = proBatch.Id,
                LeftImage = model.LeftImage,
                RightImage = model.RightImage,
            };
            return entity;
        }

        #region Validation
        public ValidationData ValidateQCMessage(
            QCEventMessage model)
        {
            var defectTypeService = provider.GetRequiredService<DefectTypeService>();
            var validationData = new ValidationData();
            var existed = QCEvents.Exists(model.Id);
            if (existed)
                return validationData.Fail(mess: "Existed ID", AppResultCode.FailValidation);
            if (string.IsNullOrWhiteSpace(model.Identifier))
                return validationData.Fail(mess: "Identifier must not be null", AppResultCode.FailValidation);
            var defectType = model.QCDefectCode == null ? null :
                defectTypeService.DefectTypes.QCMappingCode(model.QCDefectCode)
                .Select(o => new DefectType
                {
                    Id = o.Id,
                    Name = o.Name,
                    Code = o.Code,
                    QCMappingCode = o.QCMappingCode
                }).FirstOrDefault();
            if (model.QCDefectCode != null && defectType == null)
                return validationData.Fail(mess: "Invalid defect type code", AppResultCode.FailValidation);
            else validationData.TempData[nameof(defectType)] = defectType;
            return validationData;
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
