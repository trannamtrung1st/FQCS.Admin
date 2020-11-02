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
    [Route(Business.Constants.ApiEndpoint.APP_CONFIG_API)]
    [ApiController]
    [InjectionFilter]
    public class AppConfigsController : BaseController
    {
        [Inject]
        private readonly AppConfigService _service;
        [Inject]
        private readonly AppEventService _ev_service;
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        [Authorize]
        [HttpGet("")]
        public async Task<IActionResult> Get([FromQuery][QueryObject]AppConfigQueryFilter filter,
            [FromQuery]AppConfigQuerySort sort,
            [FromQuery]AppConfigQueryProjection projection,
            [FromQuery]AppConfigQueryPaging paging,
            [FromQuery]AppConfigQueryOptions options)
        {
            var validationData = _service.ValidateGetAppConfigs(
                User, filter, sort, projection, paging, options);
            if (!validationData.IsValid)
                return BadRequest(AppResult.FailValidation(data: validationData));
            var result = await _service.QueryAppConfigDynamic(
                projection, options, filter, sort, paging);
            if (options.single_only && result == null)
                return NotFound(AppResult.NotFound());
            return Ok(AppResult.Success(result));
        }

        [Authorize]
        [HttpPost("")]
        public IActionResult Create(CreateAppConfigModel model)
        {
            var validationData = _service.ValidateCreateAppConfig(User, model);
            if (!validationData.IsValid)
                return BadRequest(AppResult.FailValidation(data: validationData));
            var entity = _service.CreateAppConfig(model);
            context.SaveChanges();
            // must be in transaction
            var ev = _ev_service.CreateAppConfig(entity, User);
            context.SaveChanges();
            return Created($"/{Business.Constants.ApiEndpoint.RESOURCE_API}?id={entity.Id}",
                AppResult.Success(entity.Id));
        }

        [Authorize]
        [HttpPatch("{id}")]
        public IActionResult Update(string id, UpdateAppConfigModel model)
        {
            var entity = _service.AppConfigs.Id(id).FirstOrDefault();
            if (entity == null)
                return NotFound(AppResult.NotFound());
            var validationData = _service.ValidateUpdateAppConfig(User, entity, model);
            if (!validationData.IsValid)
                return BadRequest(AppResult.FailValidation(data: validationData));
            _service.UpdateAppConfig(entity, model);
            context.SaveChanges();
            // must be in transaction
            var ev = _ev_service.UpdateAppConfig(entity, User);
            context.SaveChanges();
            return NoContent();
        }

        [Authorize]
        [HttpPost("default")]
        public IActionResult ChangeDefaultAppConfig(ChangeDefaultConfigModel model)
        {
            var entity = _service.AppConfigs.Id(model.ConfigId).FirstOrDefault();
            if (entity == null)
                return NotFound(AppResult.NotFound());
            var validationData = _service.ValidateChangeDefaultConfig(User, entity, model);
            if (!validationData.IsValid)
                return BadRequest(AppResult.FailValidation(data: validationData));
            var oldDefault = _service.AppConfigs.IsDefault().FirstOrDefault();
            _service.ChangeDefaultConfig(entity, oldDefault);
            context.SaveChanges();
            // must be in transaction
            var ev = _ev_service.ChangeDefaultAppConfig(entity, User);
            context.SaveChanges();
            return NoContent();
        }

        [Authorize]
        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            try
            {
                var entity = _service.AppConfigs.Id(id).FirstOrDefault();
                if (entity == null)
                    return NotFound(AppResult.NotFound());
                var validationData = _service.ValidateDeleteAppConfig(User, entity);
                if (!validationData.IsValid)
                    return BadRequest(AppResult.FailValidation(data: validationData));
                _service.DeleteAppConfig(entity);
                context.SaveChanges();
                // must be in transaction
                var ev = _ev_service.DeleteAppConfig(entity, User);
                context.SaveChanges();
                return NoContent();
            }
            catch (DbUpdateException e)
            {
                _logger.Error(e);
                return BadRequest(AppResult.DependencyDeleteFail());
            }
        }

    }
}