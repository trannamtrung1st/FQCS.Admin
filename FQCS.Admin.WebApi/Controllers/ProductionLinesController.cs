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
    [Route(Business.Constants.ApiEndpoint.PRODUCTION_LINE_API)]
    [ApiController]
    [InjectionFilter]
    public class ProductionLinesController : BaseController
    {
        [Inject]
        private readonly ProductionLineService _service;
        [Inject]
        private readonly AppEventService _ev_service;
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        [Authorize]
        [HttpGet("")]
        public async Task<IActionResult> Get([FromQuery][QueryObject]ProductionLineQueryFilter filter,
            [FromQuery]ProductionLineQuerySort sort,
            [FromQuery]ProductionLineQueryProjection projection,
            [FromQuery]ProductionLineQueryPaging paging,
            [FromQuery]ProductionLineQueryOptions options)
        {
            var validationData = _service.ValidateGetProductionLines(
                User, filter, sort, projection, paging, options);
            if (!validationData.IsValid)
                return BadRequest(AppResult.FailValidation(data: validationData));
            var result = await _service.QueryProductionLineDynamic(
                projection, options, filter, sort, paging);
            if (options.single_only && result == null)
                return NotFound(AppResult.NotFound());
            return Ok(AppResult.Success(result));
        }

        [Authorize]
        [HttpPost("")]
        public IActionResult Create(CreateProductionLineModel model)
        {
            var validationData = _service.ValidateCreateProductionLine(User, model);
            if (!validationData.IsValid)
                return BadRequest(AppResult.FailValidation(data: validationData));
            var entity = _service.CreateProductionLine(model);
            context.SaveChanges();
            // must be in transaction
            var ev = _ev_service.CreateProductionLine(entity, User);
            context.SaveChanges();
            return Created($"/{Business.Constants.ApiEndpoint.PRODUCTION_LINE_API}?id={entity.Id}",
                AppResult.Success(entity.Id));
        }

        [Authorize]
        [HttpPatch("{id}")]
        public IActionResult Update(int id, UpdateProductionLineModel model)
        {
            var entity = _service.ProductionLines.Id(id).FirstOrDefault();
            if (entity == null)
                return NotFound(AppResult.NotFound());
            var validationData = _service.ValidateUpdateProductionLine(User, entity, model);
            if (!validationData.IsValid)
                return BadRequest(AppResult.FailValidation(data: validationData));
            _service.UpdateProductionLine(entity, model);
            context.SaveChanges();
            // must be in transaction
            var ev = _ev_service.UpdateProductionLine(entity, User);
            context.SaveChanges();
            return NoContent();
        }

        [Authorize]
        [HttpPatch("{id}/status")]
        public IActionResult ChangeStatus(int id, ChangeProductionLineStatusModel model)
        {
            var entity = _service.ProductionLines.Id(id).FirstOrDefault();
            if (entity == null)
                return NotFound(AppResult.NotFound());
            var validationData = _service.ValidateChangeProductionLineStatus(User, entity, model);
            if (!validationData.IsValid)
                return BadRequest(AppResult.FailValidation(data: validationData));
            _service.ChangeProductionLineStatus(entity, model);
            context.SaveChanges();
            // must be in transaction
            var ev = _ev_service.ChangeProductionLineStatus(entity, User);
            context.SaveChanges();
            return NoContent();
        }

        [Authorize]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                var entity = _service.ProductionLines.Id(id).FirstOrDefault();
                if (entity == null)
                    return NotFound(AppResult.NotFound());
                var validationData = _service.ValidateDeleteProductionLine(User, entity);
                if (!validationData.IsValid)
                    return BadRequest(AppResult.FailValidation(data: validationData));
                _service.DeleteProductionLine(entity);
                context.SaveChanges();
                // must be in transaction
                var ev = _ev_service.DeleteProductionLine(entity, User);
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