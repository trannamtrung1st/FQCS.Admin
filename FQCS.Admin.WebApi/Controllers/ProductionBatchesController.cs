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
    [Route(Business.Constants.ApiEndpoint.PRODUCTION_BATCH_API)]
    [ApiController]
    [InjectionFilter]
    public class ProductionBatchsController : BaseController
    {
        [Inject]
        private readonly ProductionBatchService _service;
        [Inject]
        private readonly AppEventService _ev_service;
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        [Authorize]
        [HttpGet("")]
        public async Task<IActionResult> Get([FromQuery][QueryObject]ProductionBatchQueryFilter filter,
            [FromQuery]ProductionBatchQuerySort sort,
            [FromQuery]ProductionBatchQueryProjection projection,
            [FromQuery]ProductionBatchQueryPaging paging,
            [FromQuery]ProductionBatchQueryOptions options)
        {
            var validationData = _service.ValidateGetProductionBatchs(
                User, filter, sort, projection, paging, options);
            if (!validationData.IsValid)
                return BadRequest(AppResult.FailValidation(data: validationData));
            var result = await _service.QueryProductionBatchDynamic(
                projection, options, filter, sort, paging);
            if (options.single_only && result == null)
                return NotFound(AppResult.NotFound());
            return Ok(AppResult.Success(result));
        }

        [Authorize]
        [HttpPost("")]
        public IActionResult Create(CreateProductionBatchModel model)
        {
            var validationData = _service.ValidateCreateProductionBatch(User, model);
            if (!validationData.IsValid)
                return BadRequest(AppResult.FailValidation(data: validationData));
            var entity = _service.CreateProductionBatch(model);
            context.SaveChanges();
            // must be in transaction
            var ev = _ev_service.CreateProductionBatch(entity, User);
            context.SaveChanges();
            return Created($"/{Business.Constants.ApiEndpoint.PRODUCTION_LINE_API}?id={entity.Id}",
                AppResult.Success(entity.Id));
        }

        [Authorize]
        [HttpPatch("{id}")]
        public IActionResult Update(int id, UpdateProductionBatchModel model)
        {
            var entity = _service.ProductionBatchs.Id(id).FirstOrDefault();
            if (entity == null)
                return NotFound(AppResult.NotFound());
            var validationData = _service.ValidateUpdateProductionBatch(User, entity, model);
            if (!validationData.IsValid)
                return BadRequest(AppResult.FailValidation(data: validationData));
            _service.UpdateProductionBatch(entity, model);
            context.SaveChanges();
            // must be in transaction
            var ev = _ev_service.UpdateProductionBatch(entity, User);
            context.SaveChanges();
            return NoContent();
        }

        [Authorize]
        [HttpPatch("{id}/status")]
        public IActionResult ChangeStatus(int id, ChangeProductionBatchStatusModel model)
        {
            var entity = _service.ProductionBatchs.Id(id).FirstOrDefault();
            if (entity == null)
                return NotFound(AppResult.NotFound());
            var validationData = _service.ValidateChangeProductionBatchStatus(User, entity, model);
            if (!validationData.IsValid)
                return BadRequest(AppResult.FailValidation(data: validationData));
            _service.ChangeProductionBatchStatus(entity, model);
            context.SaveChanges();
            // must be in transaction
            var ev = _ev_service.ChangeProductionBatchStatus(entity, User);
            context.SaveChanges();
            return NoContent();
        }

        [Authorize]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                var entity = _service.ProductionBatchs.Id(id).FirstOrDefault();
                if (entity == null)
                    return NotFound(AppResult.NotFound());
                var validationData = _service.ValidateDeleteProductionBatch(User, entity);
                if (!validationData.IsValid)
                    return BadRequest(AppResult.FailValidation(data: validationData));
                _service.DeleteProductionBatch(entity);
                context.SaveChanges();
                // must be in transaction
                var ev = _ev_service.DeleteProductionBatch(entity, User);
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