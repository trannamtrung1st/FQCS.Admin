using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FQCS.Admin.Business.Queries;
using FQCS.Admin.Business.Services;
using FQCS.Admin.Data;
using FQCS.Admin.Business.Models;
using FQCS.Admin.Data.Models;
using TNT.Core.Helpers.DI;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using System.Globalization;

namespace FQCS.Admin.ConsoleClient
{
    class Program
    {
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();
        static int id = 2;
        static void Main(string[] args)
        {
            var services = new ServiceCollection();
            Data.Global.Init(services);
            Business.Global.Init(services);
            services.AddServiceInjection();
            var provider = services.BuildServiceProvider();
            Create(provider);
            //Update(provider);
            //Delete(provider);
            Query(provider);
        }

        static void Create(IServiceProvider provider)
        {
            using (var scope = provider.CreateScope())
            {
                provider = scope.ServiceProvider;
                var context = provider.GetRequiredService<DbContext>();
                var resourceService = provider.GetRequiredService<ResourceService>();
                var model = new CreateResourceModel
                {
                    Name = "FQCS"
                };
                var entity = resourceService.CreateResource(model);
                context.SaveChanges();
                Console.WriteLine(entity.Id);
                _logger.Info("Create resource");
            }
        }

        static void Update(IServiceProvider provider)
        {
            using (var scope = provider.CreateScope())
            {
                provider = scope.ServiceProvider;
                var context = provider.GetRequiredService<DbContext>();
                var resourceService = provider.GetRequiredService<ResourceService>();
                var entity = resourceService.Resources.Id(id).SingleOrDefault();
                var model = new UpdateResourceModel()
                {
                    Name = "FQCS updated"
                };
                resourceService.UpdateResource(entity, model);
                context.SaveChanges();
                Console.WriteLine(entity.Id);
                _logger.Info("Update resource");
            }
        }

        static void Delete(IServiceProvider provider)
        {
            using (var scope = provider.CreateScope())
            {
                provider = scope.ServiceProvider;
                var context = provider.GetRequiredService<DbContext>();
                var resourceService = provider.GetRequiredService<ResourceService>();
                var exists = resourceService.Resources.Exists(id);
                if (!exists) return;
                resourceService.DeleteResource(new Resource
                {
                    Id = id
                });
                context.SaveChanges();
                _logger.Info("Delete resource");
            }
        }

        static void Query(IServiceProvider provider)
        {
            using (var scope = provider.CreateScope())
            {
                provider = scope.ServiceProvider;
                var resourceService = provider.GetRequiredService<ResourceService>();
                var results = resourceService.QueryResourceDynamic(new ResourceQueryProjection()
                {
                    fields =
                    $"{ResourceQueryProjection.INFO}"
                }, new ResourceQueryOptions() { count_total = true },
                new ResourceQueryFilter()
                {
                    //name_contains = "TNT"
                }, new ResourceQuerySort()
                {
                    sorts = "d" + ResourceQuerySort.NAME
                }, new ResourceQueryPaging()
                {
                    limit = 100,
                    page = 1
                }).Result;

                Console.WriteLine(JsonConvert.SerializeObject(results, Formatting.Indented));
                _logger.Info("Query resource data");
            }
        }
    }
}
