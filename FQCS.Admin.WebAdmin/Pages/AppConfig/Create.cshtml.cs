using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FQCS.Admin.WebAdmin.Models;
using TNT.Core.Http.DI;

namespace FQCS.Admin.WebAdmin.Pages.AppConfig
{
    [InjectionFilter]
    public class CreateModel : BasePageModel<CreateModel>
    {
        public void OnGet()
        {
            SetPageInfo();
        }

        protected override void SetPageInfo()
        {
            Info = new PageInfo
            {
                Title = "Create app config",
                Menu = Constants.Menu.APP_CONFIG,
                BackUrl = BackUrl ?? Constants.Routing.APP_CONFIG
            };
        }
    }
}
