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

namespace FQCS.Admin.Business.Services
{
    public class ReportService : Service
    {
        [Inject]
        protected readonly QCEventService qcEventService;
        [Inject]
        protected readonly ProductionBatchService proBatchService;

        public ReportService(ServiceInjection inj) : base(inj)
        {
        }

        public XLWorkbook GenerateBatchEventReport(BatchReportOptions options)
        {
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
            var entities = qcEventService.QCEvents.OfBatch(options.batch_id)
                .SortByTime(false)
                .Select(o => new
                {
                    o.Id,
                    o.CreatedTime,
                    DefectTypeCode = o.DefectType != null ? o.DefectType.Code : null,
                    DefectTypeName = o.DefectType != null ? o.DefectType.Name : null,
                    BatchCode = o.Batch.Code
                }).ToList();
            var no = 1;
            foreach (var o in entities)
                sheet1.SetRowData(currentRow++, no++, o.Id, o.CreatedTime, o.DefectTypeCode, o.DefectTypeName, o.BatchCode);
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
            var groups = entities.GroupBy(o => new
            {
                o.DefectTypeCode,
                o.DefectTypeName
            }).ToList();
            no = 1;
            foreach (var g in groups)
            {
                var count = g.Count();
                var avg = Math.Round((double)count / proBatch.TotalAmount * 100, 2);
                sheet2.SetRowData(currentRow++, no++, g.Key.DefectTypeCode ?? "", g.Key.DefectTypeName ?? "", count, avg);
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
