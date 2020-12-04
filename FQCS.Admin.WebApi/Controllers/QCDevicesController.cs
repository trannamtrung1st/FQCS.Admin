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
using System.Net.Mime;

namespace FQCS.Admin.WebApi.Controllers
{
    [Route(Business.Constants.ApiEndpoint.QC_DEVICE_API)]
    [ApiController]
    [InjectionFilter]
    public class QCDevicesController : BaseController
    {
        [Inject]
        private readonly IQCDeviceService _service;
        [Inject]
        private readonly IAppEventService _ev_service;
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        [Authorize]
        [HttpGet("")]
        public async Task<IActionResult> Get([FromQuery][QueryObject]QCDeviceQueryFilter filter,
            [FromQuery]QCDeviceQuerySort sort,
            [FromQuery]QCDeviceQueryProjection projection,
            [FromQuery]QCDeviceQueryPaging paging,
            [FromQuery]QCDeviceQueryOptions options)
        {
            var validationData = _service.ValidateGetQCDevices(
                User, filter, sort, projection, paging, options);
            if (!validationData.IsValid)
                return BadRequest(AppResult.FailValidation(data: validationData));
            var result = await _service.QueryQCDeviceDynamic(
                projection, options, filter, sort, paging);
            if (options.single_only && result == null)
                return NotFound(AppResult.NotFound());
            return Ok(AppResult.Success(result));
        }

        [Authorize]
        [HttpPost("")]
        public IActionResult Create(CreateQCDeviceModel model)
        {
            var validationData = _service.ValidateCreateQCDevice(User, model);
            if (!validationData.IsValid)
                return BadRequest(AppResult.FailValidation(data: validationData));
            var entity = _service.CreateQCDevice(model);
            context.SaveChanges();
            // must be in transaction
            var ev = _ev_service.CreateQCDevice(entity, User);
            context.SaveChanges();
            return Created($"/{Business.Constants.ApiEndpoint.PRODUCTION_LINE_API}?id={entity.Id}",
                AppResult.Success(entity.Id));
        }

        [Authorize]
        [HttpPatch("{id}")]
        public IActionResult Update(int id, UpdateQCDeviceModel model)
        {
            var entity = _service.QCDevices.Id(id).FirstOrDefault();
            if (entity == null)
                return NotFound(AppResult.NotFound());
            var validationData = _service.ValidateUpdateQCDevice(User, entity, model);
            if (!validationData.IsValid)
                return BadRequest(AppResult.FailValidation(data: validationData));
            _service.UpdateQCDevice(entity, model);
            context.SaveChanges();
            // must be in transaction
            var ev = _ev_service.UpdateQCDevice(entity, User);
            context.SaveChanges();
            return NoContent();
        }

        [Authorize]
        [HttpPatch("{id}/status")]
        public IActionResult ChangeStatus(int id, ChangeQCDeviceStatusModel model)
        {
            var entity = _service.QCDevices.Id(id).FirstOrDefault();
            if (entity == null)
                return NotFound(AppResult.NotFound());
            var validationData = _service.ValidateChangeQCDeviceStatus(User, entity, model);
            if (!validationData.IsValid)
                return BadRequest(AppResult.FailValidation(data: validationData));
            _service.ChangeQCDeviceStatus(entity, model);
            context.SaveChanges();
            // must be in transaction
            var ev = _ev_service.ChangeQCDeviceStatus(entity, User);
            context.SaveChanges();
            return NoContent();
        }

        [Authorize]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                var entity = _service.QCDevices.Id(id).FirstOrDefault();
                if (entity == null)
                    return NotFound(AppResult.NotFound());
                var validationData = _service.ValidateDeleteQCDevice(User, entity);
                if (!validationData.IsValid)
                    return BadRequest(AppResult.FailValidation(data: validationData));
                _service.DeleteQCDevice(entity);
                context.SaveChanges();
                // must be in transaction
                var ev = _ev_service.DeleteQCDevice(entity, User);
                context.SaveChanges();
                return NoContent();
            }
            catch (DbUpdateException e)
            {
                _logger.Error(e);
                return BadRequest(AppResult.DependencyDeleteFail());
            }
        }

        [Authorize]
        [HttpPost("cmd")]
        public async Task<IActionResult> SendCommandToDeviceAPI(SendCommandToDeviceAPIModel model)
        {
            var entity = _service.QCDevices.Id(model.DeviceId).FirstOrDefault();
            if (entity == null)
                return NotFound(AppResult.NotFound());
            var validationData = _service.ValidateSendCommandToDeviceAPI(User, model);
            if (!validationData.IsValid)
                return BadRequest(AppResult.FailValidation(data: validationData));
            object respData = null;
            switch (model.Command)
            {
                case Business.Constants.QCEventOps.GET_EVENTS:
                    var (succ, fail, latest) = await _service.SendCommandGetEvents(model, entity, entity.Config);
                    respData = new
                    {
                        success = succ,
                        fail = fail
                    };
                    context.SaveChanges();
                    if (latest != null)
                        await _service.SendCommandUpdateLastEventTime(new UpdateLastEventTimeModel
                        {
                            UtcTime = latest.Value
                        }, entity, entity.Config);
                    break;
                case Business.Constants.QCEventOps.DOWNLOAD_IMAGES:
                    var (stream, fileName) = await _service.SendCommandDownloadImages(
                        model, entity, entity.Config);
                    return File(stream, MediaTypeNames.Application.Zip, fileName);
                case Business.Constants.QCEventOps.CLEAR_ALL:
                    var deleted = await _service.SendCommandClearAllEvents(
                        model, entity, entity.Config);
                    respData = deleted;
                    break;
                case Business.Constants.QCEventOps.TRIGGER_SEND:
                    var dateTimeOffset = await _service.SendCommandTriggerSendUnsent(
                        model, entity, entity.Config);
                    respData = dateTimeOffset;
                    break;
            }
            // must be in transaction
            var ev = _ev_service.SendCommandToDeviceAPI(model, entity, User);
            context.SaveChanges();
            return Ok(AppResult.Success(respData));
        }


    }
}