﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using FQCS.Admin.Business;
using FQCS.Admin.Business.Services;
using FQCS.Admin.Data.Models;
using FQCS.Admin.WebAdmin.Pages.Shared;
using TNT.Core.Helpers.DI;

namespace FQCS.Admin.WebAdmin
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            configuration.Bind("BusinessSettings", Business.Settings.Instance);
            configuration.Bind("WebAdminSettings", WebAdmin.Settings.Instance);
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            ServiceInjection.Register(new List<Assembly>()
            {
                Assembly.GetExecutingAssembly()
            });
            services.AddServiceInjection();
            var connStr = Configuration.GetConnectionString("DataContext");
#if TEST
            connStr = connStr.Replace("{envConfig}", ".Test");
#else
            connStr = connStr.Replace("{envConfig}", "");
#endif
            services.AddDbContext<DataContext>(options =>
            {
                options.UseSqlServer(connStr);
            });
            Data.Global.Init(services);
            Business.Global.Init(services);
            #region OAuth
            //for some default Identity configuration, include AddAuthentication (IdentityConstants)
            services.AddIdentity<AppUser, AppRole>(options =>
            {
                options.SignIn.RequireConfirmedEmail = false;
            })
            .AddEntityFrameworkStores<DataContext>();
            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 0;

                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                options.User.AllowedUserNameCharacters =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = false;
            });
            #endregion
            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.HttpOnly = true;
                options.AccessDeniedPath = Constants.Routing.ACCESS_DENIED;
                options.ExpireTimeSpan = TimeSpan.FromHours(
                    WebAdmin.Settings.Instance.CookiePersistentHours);
                options.LoginPath = Constants.Routing.LOGIN;
                options.LogoutPath = Constants.Routing.LOGOUT;
                options.ReturnUrlParameter = "return_url";
                options.SlidingExpiration = true;
                options.Events.OnValidatePrincipal = async (c) =>
                {
                    var identity = c.Principal.Identity as ClaimsIdentity;
                    //extra claims will be expired after amount of time
                    if (identity.FindFirst(Business.Constants.AppClaimType.UserName)?.Value == null)
                    {
                        var identityService = c.HttpContext.RequestServices.GetRequiredService<IIdentityService>();
                        var entity = await identityService.GetUserByUserNameAsync(identity.Name);
                        var extraClaims = identityService.GetExtraClaims(entity);
                        identity.AddClaims(extraClaims);
                        c.ShouldRenew = true;
                    }
                    await SecurityStampValidator.ValidatePrincipalAsync(c);
                };
            });
            services.AddScoped<Layout>();
            services.AddControllers();
            services.AddRazorPages(options =>
            {
                var allowAnnonymousPages = new[] {
                    "/AccessDenied", "/Error", "/Status", "/Identity/Login", "/Identity/Register" };
                var authorizeFolders = new[] { "/" };
                options.Conventions
                    .AddPageRoute("/Resource/Detail", Constants.Routing.RESOURCE_DETAIL)
                    .AddPageRoute("/ProductionLine/Detail", Constants.Routing.PRODUCTION_LINE_DETAIL)
                    .AddPageRoute("/ProductModel/Detail", Constants.Routing.PRODUCT_MODEL_DETAIL)
                    .AddPageRoute("/QCDevice/Detail", Constants.Routing.QC_DEVICE_DETAIL)
                    .AddPageRoute("/ProductionBatch/Detail", Constants.Routing.PRODUCTION_BATCH_DETAIL)
                    .AddPageRoute("/QCEvent/Detail", Constants.Routing.QC_EVENT_DETAIL)
                    .AddPageRoute("/AppConfig/Detail", Constants.Routing.APP_CONFIG_DETAIL)
                    .AddPageRoute("/DefectType/Detail", Constants.Routing.DEFECT_TYPE_DETAIL);
                foreach (var f in authorizeFolders)
                    options.Conventions.AuthorizeFolder(f);
                foreach (var p in allowAnnonymousPages)
                    options.Conventions.AllowAnonymousToPage(p);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseExceptionHandler(Constants.Routing.ERROR);
            app.UseStatusCodePagesWithReExecute(Constants.Routing.STATUS, "?code={0}");
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapRazorPages();
            });
        }
    }
}
