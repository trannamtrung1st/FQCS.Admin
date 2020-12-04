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
using Microsoft.AspNetCore.Http;
using System.IO;
using ClosedXML.Excel;
using Microsoft.Extensions.DependencyInjection;

namespace FQCS.Admin.Business.Services
{
    public interface IReportService
    {
        XLWorkbook GenerateBatchEventReport(BatchReportOptions options);
        byte[] SaveAsBytes(XLWorkbook workbook);
        ValidationData ValidateGetBatchReport(ClaimsPrincipal principal, BatchReportOptions options);
    }

    public class ReportService : Service, IReportService
    {
        public ReportService(ServiceInjection inj) : base(inj)
        {
        }

        public XLWorkbook GenerateBatchEventReport(BatchReportOptions options)
        {
            var proBatchService = provider.GetRequiredService<IProductionBatchService>();
            var qcEventService = provider.GetRequiredService<IQCEventService>();
            var defectTypeService = provider.GetRequiredService<IDefectTypeService>();
            var proBatch = proBatchService.ProductionBatchs.Id(options.batch_id)
                .Select(o => new ProductionBatch
                {
                    Id = o.Id,
                    Code = o.Code,
                    TotalAmount = o.TotalAmount
                }).First();
            var workbook = new XLWorkbook();
            // sheet 1
            var sheet1 = workbook.Worksheets.Add("Batch report");
            var currentRow = 1;
            var headerTitle = new[] { "No", "Id", "Time", "Defect code", "Defect name", "Batch code" };
            var header = sheet1.SetRowData(currentRow++, headerTitle);
            header.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            header.Style.Font.SetBold();
            var defectTypesMap = defectTypeService.DefectTypes.Select(o => new DefectType
            {
                Id = o.Id,
                QCMappingCode = o.QCMappingCode,
                Code = o.Code,
                Name = o.Name,
            }).ToDictionary(o => o.Id);
            var defRecords = qcEventService.QCEvents.OfBatch(options.batch_id)
                .SortByTime(false)
                .Join(qcEventService.QCEventDetails, o => o.Id, o => o.EventId, (ev, dt) => new
                {
                    ev.Id,
                    ev.CreatedTime,
                    dt.DefectTypeId,
                    BatchCode = ev.Batch.Code
                }).ToList();
            var defRecordsFinal = defRecords.Select(o => new
            {
                o.Id,
                o.CreatedTime,
                o.BatchCode,
                DefectTypeName = defectTypesMap[o.DefectTypeId].Name,
                DefectTypeCode = defectTypesMap[o.DefectTypeId].Code,
            });
            var passRecords = qcEventService.QCEvents.OfBatch(options.batch_id)
                .Passed()
                .SortByTime(false)
                .Select(o => new
                {
                    o.Id,
                    o.CreatedTime,
                    BatchCode = o.Batch.Code,
                    DefectTypeName = "",
                    DefectTypeCode = "",
                }).ToList();
            var no = 1;
            IEnumerable<dynamic> allRecords = new List<dynamic>();
            allRecords = allRecords.Concat(defRecordsFinal).Concat(passRecords).ToList();
            foreach (var o in allRecords)
                Business.Helpers.XLHelper.SetRowData(sheet1, currentRow++,
                    no++, o.Id, o.CreatedTime, o.DefectTypeCode, o.DefectTypeName, o.BatchCode);

            for (var i = 1; i <= headerTitle.Length; i++)
                sheet1.Column(i).AdjustToContents();

            // sheet 2
            var sheet2 = workbook.AddWorksheet("Defect report");
            currentRow = 1;
            headerTitle = new[] { "No", "Defect code", "Defect name", "Amount", "Average(%)",
                "", "Batch code", proBatch.Code };
            var row = sheet2.SetRowData(currentRow++, headerTitle);
            var totalCell = sheet2.Cell(2, 7);
            totalCell.Value = "Total amount"; totalCell.Style.Font.SetBold();
            sheet2.Cell(2, 8).Value = proBatch.TotalAmount;

            row.Style.Font.SetBold();
            row.Cell(headerTitle.Length).Style.Font.SetBold(false);
            var groups = allRecords.GroupBy(o => new
            {
                o.DefectTypeCode,
                o.DefectTypeName
            }).ToList();
            no = 1;
            foreach (var g in groups)
            {
                var count = g.Count();
                var avg = Math.Round((double)count / proBatch.TotalAmount * 100, 2);
                Business.Helpers.XLHelper.SetRowData(sheet2, currentRow++, no++,
                    g.Key.DefectTypeCode ?? "", g.Key.DefectTypeName ?? "", count, avg);
            }
            for (var i = 1; i <= headerTitle.Length; i++)
                sheet2.Column(i).AdjustToContents();

            return workbook;
        }

        public byte[] SaveAsBytes(XLWorkbook workbook)
        {
            using (var memStream = new MemoryStream())
            {
                workbook.SaveAs(memStream);
                memStream.Position = 0;
                return memStream.ToArray();
            }
        }

        #region Validation
        public ValidationData ValidateGetBatchReport(
            ClaimsPrincipal principal, BatchReportOptions options)
        {
            return new ValidationData();
        }
        #endregion
    }
}
