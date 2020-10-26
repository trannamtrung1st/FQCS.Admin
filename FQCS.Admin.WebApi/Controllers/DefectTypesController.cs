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
    [Route(Business.Constants.ApiEndpoint.DEFECT_TYPE_API)]
    [ApiController]
    [InjectionFilter]
    public class DefectTypesController : BaseController
    {
        [Inject]
        private readonly DefectTypeService _service;
        [Inject]
        private readonly AppEventService _ev_service;
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        [Authorize]
        [HttpGet("")]
        public async Task<IActionResult> Get([FromQuery][QueryObject]DefectTypeQueryFilter filter,
            [FromQuery]DefectTypeQuerySort sort,
            [FromQuery]DefectTypeQueryProjection projection,
            [FromQuery]DefectTypeQueryPaging paging,
            [FromQuery]DefectTypeQueryOptions options)
        {
            var validationData = _service.ValidateGetDefectTypes(
                User, filter, sort, projection, paging, options);
            if (!validationData.IsValid)
                return BadRequest(AppResult.FailValidation(data: validationData));
            var result = await _service.QueryDefectTypeDynamic(
                projection, options, filter, sort, paging);
            if (options.single_only && result == null)
                return NotFound(AppResult.NotFound());
            return Ok(AppResult.Success(result));
        }

        [Authorize]
        [HttpPost("")]
        public IActionResult Create(CreateDefectTypeModel model)
        {
            var validationData = _service.ValidateCreateDefectType(User, model);
            if (!validationData.IsValid)
                return BadRequest(AppResult.FailValidation(data: validationData));
            var entity = _service.CreateDefectType(model);
            context.SaveChanges();
            // must be in transaction
            var ev = _ev_service.CreateDefectType(entity, User);
            context.SaveChanges();
            return Created($"/{Business.Constants.ApiEndpoint.RESOURCE_API}?id={entity.Id}",
                AppResult.Success(entity.Id));
        }

        [Authorize]
        [HttpPatch("{id}")]
        public IActionResult Update(int id, UpdateDefectTypeModel model)
        {
            var entity = _service.DefectTypes.Id(id).FirstOrDefault();
            if (entity == null)
                return NotFound(AppResult.NotFound());
            var validationData = _service.ValidateUpdateDefectType(User, entity, model);
            if (!validationData.IsValid)
                return BadRequest(AppResult.FailValidation(data: validationData));
            _service.UpdateDefectType(entity, model);
            context.SaveChanges();
            // must be in transaction
            var ev = _ev_service.UpdateDefectType(entity, User);
            context.SaveChanges();
            return NoContent();
        }

        [Authorize]
        [HttpPost("{id}/image")]
        public async Task<IActionResult> UpdateImage(int id,
            [FromForm]UpdateDefectTypeImageModel model)
        {
            var entity = _service.DefectTypes.Id(id).FirstOrDefault();
            if (entity == null)
                return NotFound(AppResult.NotFound());
            var validationData = _service.ValidateUpdateDefectTypeImage(User, entity, model);
            if (!validationData.IsValid)
                return BadRequest(AppResult.FailValidation(data: validationData));
            var (relPath, fullPath) = _service.GetDefectTypeImagePath(entity,
                Settings.Instance.UploadFolderPath, Settings.Instance.WebRootPath);
            var oldRelPath = entity.SampleImage;
            _service.UpdateDefectTypeImage(entity, relPath);
            context.SaveChanges();
            // must be in transaction
            var ev = _ev_service.UpdateDefectTypeImage(entity, User);
            context.SaveChanges();
            await _service.SaveReplaceDefectTypeImage(model, fullPath, Settings.Instance.WebRootPath, oldRelPath);
            return NoContent();
        }

        [Authorize]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                var entity = _service.DefectTypes.Id(id).FirstOrDefault();
                if (entity == null)
                    return NotFound(AppResult.NotFound());
                var validationData = _service.ValidateDeleteDefectType(User, entity);
                if (!validationData.IsValid)
                    return BadRequest(AppResult.FailValidation(data: validationData));
                _service.DeleteDefectType(entity, Settings.Instance.UploadFolderPath, Settings.Instance.WebRootPath);
                context.SaveChanges();
                // must be in transaction
                var ev = _ev_service.DeleteDefectType(entity, User);
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