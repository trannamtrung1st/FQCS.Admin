using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using FQCS.Admin.WebAdmin.Models;
using TNT.Core.Http.DI;

namespace FQCS.Admin.WebAdmin.Pages.ProductionBatch
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
                Title = "Production batch detail",
                Menu = Constants.Menu.PRODUCTION_BATCH,
                BackUrl = BackUrl ?? Constants.Routing.PRODUCTION_BATCH
            };
        }
    }
}
