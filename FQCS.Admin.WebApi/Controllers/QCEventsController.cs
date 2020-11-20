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

namespace FQCS.Admin.WebApi.Controllers
{
    [Route(Business.Constants.ApiEndpoint.QC_EVENT_API)]
    [ApiController]
    [InjectionFilter]
    public class QCEventsController : BaseController
    {
        [Inject]
        private readonly QCEventService _service;
        [Inject]
        private readonly AppEventService _ev_service;
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        [Authorize]
        [HttpGet("")]
        public async Task<IActionResult> Get([FromQuery][QueryObject]QCEventQueryFilter filter,
            [FromQuery]QCEventQuerySort sort,
            [FromQuery]QCEventQueryProjection projection,
            [FromQuery]QCEventQueryPaging paging,
            [FromQuery]QCEventQueryOptions options)
        {
            var validationData = _service.ValidateGetQCEvents(
                User, filter, sort, projection, paging, options);
            if (!validationData.IsValid)
                return BadRequest(AppResult.FailValidation(data: validationData));
            var result = await _service.QueryQCEventDynamic(
                projection, options, Settings.Instance.QCEventImageFolderPath, filter, sort, paging);
            if (options.single_only && result == null)
                return NotFound(AppResult.NotFound());
            return Ok(AppResult.Success(result));
        }


        [Authorize]
        [HttpPut("seen-status")]
        public IActionResult UpdateSeenStatus([FromQuery][QueryObject]QCEventQueryFilter filter,
            [FromQuery]QCEventQuerySort sort,
            [FromQuery]QCEventQueryPaging paging,
            [FromQuery]QCEventQueryOptions options)
        {
            var validationData = _service.ValidateUpdateSeenStatus(
                User, filter, sort, paging, options);
            if (!validationData.IsValid)
                return BadRequest(AppResult.FailValidation(data: validationData));
            var query = _service.GetQueryableQCEventForUpdate(options, filter, sort, paging);
            var updated = _service.UpdateEventsSeenStatus(query, true);
            return Ok(AppResult.Success(updated));
        }


    }
}