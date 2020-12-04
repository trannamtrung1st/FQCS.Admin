using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TNT.Core.Http.DI;
using TNT.Core.Helpers.DI;
using System.Data.SqlClient;
using FQCS.Admin.Business.Services;
using FQCS.Admin.Business.Models;
using FQCS.Admin.Business.Queries;
using Microsoft.EntityFrameworkCore;
using FQCS.Admin.Business;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using FQCS.Admin.Business.Helpers;
using ClosedXML.Extensions;
using System.Net.Mime;

namespace FQCS.Admin.WebApi.Controllers
{
    [Route(Business.Constants.ApiEndpoint.REPORT_API)]
    [ApiController]
    [InjectionFilter]
    public class ReportsController : BaseController
    {
        [Inject]
        private readonly IReportService _service;
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

#if RELEASE
        [Authorize]
#endif
        [HttpGet("batch")]
        public IActionResult GetBatchReport(
            [FromQuery][QueryObject]BatchReportOptions options)
        {
            var validationData = _service.ValidateGetBatchReport(User, options);
            if (!validationData.IsValid)
                return BadRequest(AppResult.FailValidation(data: validationData));
            using var workbook = _service.GenerateBatchEventReport(options);
            var data = _service.SaveAsBytes(workbook);
            return File(data, Business.Constants.ContentType.SPREADSHEET, $"batch-{options.batch_id}-report.xlsx");
        }

    }
}