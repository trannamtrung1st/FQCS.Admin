using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using FQCS.Admin.WebAdmin.Models;
using TNT.Core.Http.DI;

namespace FQCS.Admin.WebAdmin.Pages.QCDevice
{
    [InjectionFilter]
    public class DetailModel : BasePageModel<DetailModel>
    {
        public int Id { get; set; }
        public void OnGet(int id)
        {
            SetPageInfo();
            Id = id;
        }

        protected override void SetPageInfo()
        {
            Info = new PageInfo
            {
                Title = "QC device detail",
                Menu = Constants.Menu.QC_DEVICE,
                BackUrl = BackUrl ?? Constants.Routing.QC_DEVICE
            };
        }
    }
}
