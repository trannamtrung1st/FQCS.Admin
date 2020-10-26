using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FQCS.Admin.WebAdmin.Models;
using TNT.Core.Http.DI;

namespace FQCS.Admin.WebAdmin.Pages.DefectType
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
                Title = "Defect type",
                Menu = Constants.Menu.DEFECT_TYPE,
                BackUrl = BackUrl ?? Constants.Routing.DASHBOARD
            };
        }
    }
}
