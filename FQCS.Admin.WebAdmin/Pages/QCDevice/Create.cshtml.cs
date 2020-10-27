using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FQCS.Admin.WebAdmin.Models;
using TNT.Core.Http.DI;

namespace FQCS.Admin.WebAdmin.Pages.QCDevice
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
                Title = "Create QC device",
                Menu = Constants.Menu.QC_DEVICE,
                BackUrl = BackUrl ?? Constants.Routing.QC_DEVICE
            };
        }
    }
}
