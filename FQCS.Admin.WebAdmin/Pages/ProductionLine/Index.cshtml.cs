using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FQCS.Admin.WebAdmin.Models;
using TNT.Core.Http.DI;

namespace FQCS.Admin.WebAdmin.Pages.ProductionLine
{
    [InjectionFilter]
    public class IndexModel : BasePageModel<IndexModel>
    {
        public void OnGet()
        {
            SetPageInfo();
        }

        protected override void SetPageInfo()
        {
            Info = new PageInfo
            {
                Title = "Production line",
                Menu = Constants.Menu.PRODUCTION_LINE,
                BackUrl = BackUrl ?? Constants.Routing.DASHBOARD
            };
        }
    }
}
