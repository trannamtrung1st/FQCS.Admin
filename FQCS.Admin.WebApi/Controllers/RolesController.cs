﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FQCS.Admin.Business;
using FQCS.Admin.Business.Models;
using FQCS.Admin.Business.Services;
using TNT.Core.Helpers.DI;
using TNT.Core.Http.DI;

namespace FQCS.Admin.WebApi.Controllers
{
    [Route(Business.Constants.ApiEndpoint.ROLE_API)]
    [ApiController]
    [InjectionFilter]
    public class RolesController : BaseController
    {
        [Inject]
        private readonly IIdentityService _service;
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        [HttpGet("")]
        public IActionResult GetRoles()
        {
            var validationData = _service.ValidateGetRoles(User);
            if (!validationData.IsValid)
                return BadRequest(AppResult.FailValidation(data: validationData));
            var result = _service.Roles.ToList();
            return Ok(AppResult.Success(result));
        }

#if DEBUG
        [HttpPost("")]
        public async Task<IActionResult> CreateRole(CreateRoleModel model)
        {
            var validationData = _service.ValidateCreateRole(User, model);
            if (!validationData.IsValid)
                return BadRequest(AppResult.FailValidation(data: validationData));
            var result = await _service.CreateRoleAsync(model);
            if (result.Succeeded)
                return Ok(AppResult.Success(result));
            foreach (var err in result.Errors)
                ModelState.AddModelError(err.Code, err.Description);
            return BadRequest(AppResult.FailValidation(ModelState));
        }

        [HttpPatch("{name}")]
        public async Task<IActionResult> UpdateRole(string name, UpdateRoleModel model)
        {
            var validationData = _service.ValidateUpdateRole(User, model);
            if (!validationData.IsValid)
                return BadRequest(AppResult.FailValidation(data: validationData));
            var entity = _service.GetRoleByName(name);
            if (entity == null)
                return NotFound(AppResult.NotFound());
            var result = await _service.UpdateRoleAsync(entity, model);
            if (result.Succeeded)
                return Ok(AppResult.Success(entity));
            foreach (var err in result.Errors)
                ModelState.AddModelError(err.Code, err.Description);
            return BadRequest(AppResult.FailValidation(ModelState));
        }

        [HttpDelete("{name}")]
        public async Task<IActionResult> RemoveRole(string name)
        {
            var entity = _service.GetRoleByName(name);
            if (entity == null)
                return NotFound(AppResult.NotFound());
            var validationData = _service.ValidateDeleteRole(User, entity);
            if (!validationData.IsValid)
                return BadRequest(AppResult.FailValidation(data: validationData));
            var result = await _service.RemoveRoleAsync(entity);
            if (result.Succeeded)
                return Ok(AppResult.Success(result));
            foreach (var err in result.Errors)
                ModelState.AddModelError(err.Code, err.Description);
            return BadRequest(AppResult.FailValidation(ModelState));
        }
#endif
    }
}
