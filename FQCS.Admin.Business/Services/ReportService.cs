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
        public ReportService(ServiceInjection inj) : base(inj)
        {
        }

        public XLWorkbook GenerateBatchEventReport(BatchReportOptions options)
        {
            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Batch report");
            var header = worksheet.SetRowData(1, "No", "Id", "Time", "Defect code", "Defect name", "Batch code");
            header.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            header.Style.Font.SetBold();
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
