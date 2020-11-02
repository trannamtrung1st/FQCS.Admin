using System;
using System.Collections.Generic;
using System.Text;
using FQCS.Admin.Data.Models;
using TNT.Core.Helpers.DI;

namespace FQCS.Admin.Business.Services
{
    public abstract class Service
    {
        [Inject]
        protected readonly DataContext context;
        [Inject]
        protected readonly IServiceProvider provider;

        public Service(ServiceInjection inj)
        {
            inj.Inject(this);
        }

    }
}
