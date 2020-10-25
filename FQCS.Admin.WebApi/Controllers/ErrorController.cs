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
using Microsoft.AspNetCore.Diagnostics;

namespace FQCS.Admin.WebApi.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route(ApiEndpoint.ERROR)]
    [ApiController]
    [InjectionFilter]
    public class ErrorController : BaseController
    {
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        [Route("")]
        public IActionResult HandleException()
        {
            var context = HttpContext.Features.Get<IExceptionHandlerFeature>();
            if (context.Error == null) return BadRequest();
            var e = context.Error;
            _logger.Error(e);
#if DEBUG
            return Error(AppResult.Error(data: e));
#else
            return Error(AppResult.Error());
#endif
        }
    }
}