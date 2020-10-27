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
    [Route(Business.Constants.ApiEndpoint.PRODUCT_MODEL_API)]
    [ApiController]
    [InjectionFilter]
    public class ProductModelsController : BaseController
    {
        [Inject]
        private readonly ProductModelService _service;
        [Inject]
        private readonly AppEventService _ev_service;
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        [Authorize]
        [HttpGet("")]
        public async Task<IActionResult> Get([FromQuery][QueryObject]ProductModelQueryFilter filter,
            [FromQuery]ProductModelQuerySort sort,
            [FromQuery]ProductModelQueryProjection projection,
            [FromQuery]ProductModelQueryPaging paging,
            [FromQuery]ProductModelQueryOptions options)
        {
            var validationData = _service.ValidateGetProductModels(
                User, filter, sort, projection, paging, options);
            if (!validationData.IsValid)
                return BadRequest(AppResult.FailValidation(data: validationData));
            var result = await _service.QueryProductModelDynamic(
                projection, options, filter, sort, paging);
            if (options.single_only && result == null)
                return NotFound(AppResult.NotFound());
            return Ok(AppResult.Success(result));
        }

        [Authorize]
        [HttpPost("")]
        public IActionResult Create(CreateProductModelModel model)
        {
            var validationData = _service.ValidateCreateProductModel(User, model);
            if (!validationData.IsValid)
                return BadRequest(AppResult.FailValidation(data: validationData));
            var entity = _service.CreateProductModel(model);
            context.SaveChanges();
            // must be in transaction
            var ev = _ev_service.CreateProductModel(entity, User);
            context.SaveChanges();
            return Created($"/{Business.Constants.ApiEndpoint.RESOURCE_API}?id={entity.Id}",
                AppResult.Success(entity.Id));
        }

        [Authorize]
        [HttpPatch("{id}")]
        public IActionResult Update(int id, UpdateProductModelModel model)
        {
            var entity = _service.ProductModels.Id(id).FirstOrDefault();
            if (entity == null)
                return NotFound(AppResult.NotFound());
            var validationData = _service.ValidateUpdateProductModel(User, entity, model);
            if (!validationData.IsValid)
                return BadRequest(AppResult.FailValidation(data: validationData));
            _service.UpdateProductModel(entity, model);
            context.SaveChanges();
            // must be in transaction
            var ev = _ev_service.UpdateProductModel(entity, User);
            context.SaveChanges();
            return NoContent();
        }

        [Authorize]
        [HttpPost("{id}/image")]
        public async Task<IActionResult> UpdateImage(int id,
            [FromForm]UpdateProductModelImageModel model)
        {
            var entity = _service.ProductModels.Id(id).FirstOrDefault();
            if (entity == null)
                return NotFound(AppResult.NotFound());
            var validationData = _service.ValidateUpdateProductModelImage(User, entity, model);
            if (!validationData.IsValid)
                return BadRequest(AppResult.FailValidation(data: validationData));
            var (relPath, fullPath) = _service.GetProductModelImagePath(entity,
                Settings.Instance.UploadFolderPath, Settings.Instance.WebRootPath);
            var oldRelPath = entity.Image;
            _service.UpdateProductModelImage(entity, relPath);
            context.SaveChanges();
            // must be in transaction
            var ev = _ev_service.UpdateProductModelImage(entity, User);
            context.SaveChanges();
            await _service.SaveReplaceProductModelImage(model, fullPath, Settings.Instance.WebRootPath, oldRelPath);
            return Created($"/{relPath}",
                AppResult.Success(relPath));
        }

        [Authorize]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                var entity = _service.ProductModels.Id(id).FirstOrDefault();
                if (entity == null)
                    return NotFound(AppResult.NotFound());
                var validationData = _service.ValidateDeleteProductModel(User, entity);
                if (!validationData.IsValid)
                    return BadRequest(AppResult.FailValidation(data: validationData));
                _service.DeleteProductModel(entity);
                _service.DeleteProductModelFolder(entity, Settings.Instance.UploadFolderPath, Settings.Instance.WebRootPath);
                context.SaveChanges();
                // must be in transaction
                var ev = _ev_service.DeleteProductModel(entity, User);
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