using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using FQCS.Admin.WebAdmin.Models;
using TNT.Core.Http.DI;

namespace FQCS.Admin.WebAdmin.Pages.QCEvent
{
    [InjectionFilter]
    public class DetailModel : BasePageModel<DetailModel>
    {
        public string Id { get; set; }
        public void OnGet(string id)
        {
            SetPageInfo();
            Id = id;
        }

        protected override void SetPageInfo()
        {
            Info = new PageInfo
            {
                Title = "QC event detail",
                Menu = Constants.Menu.QC_EVENT,
                BackUrl = BackUrl ?? Constants.Routing.QC_EVENT
            };
        }
    }
}
