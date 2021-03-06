using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace WebSiteCoreProject1
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // See Dependency Injection here https://docs.microsoft.com/en-us/aspnet/core/mvc/controllers/dependency-injection?view=aspnetcore-2.2
        public void ConfigureServices(IServiceCollection services)
        {
            #region Session Provider
            // See https://docs.microsoft.com/en-us/aspnet/core/fundamentals/app-state?view=aspnetcore-5.0
            // Set the in-memory session provider
            services.AddDistributedMemoryCache();
            services.AddSession(options =>
            {
                //options.IdleTimeout = TimeSpan.FromSeconds(10);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
            #endregion

            #region Authentication
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
               .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
               {
                   options.LoginPath = new PathString("/Home/Login");
                   options.AccessDeniedPath = new PathString("/Account/Denied");
               });
            #endregion

            #region Setup Entity Framework
            // 1. Install Entity Framework Core Tools
            // Install-Package Microsoft.EntityFrameworkCore.Tools -Version 2.1.2
            // 2. Install the SQL Server Provider
            // Install-Package Microsoft.EntityFrameworkCore.SqlServer -Version 2.1.2
            // 3. Create the Database Model
            // Scaffold-DbContext 'Data Source=.\SQLEXPRESS;Initial Catalog=mini-cstructor;integrated security=True' Microsoft.EntityFrameworkCore.SqlServer
            // 4. (optional) Re-Create the Database Model (if DB has changed)
            // Scaffold-DbContext 'Data Source=.\SQLEXPRESS;Initial Catalog=mini-cstructor;integrated security=True' Microsoft.EntityFrameworkCore.SqlServer -Force
            #endregion

            #region Dependency Injection
            // Add Dependency Injection of DbContext.
            // See https://docs.microsoft.com/en-us/aspnet/core/mvc/controllers/dependency-injection?view=aspnetcore-2.2
            // Also https://docs.microsoft.com/en-us/ef/core/dbcontext-configuration/#dbcontext-in-dependency-injection-for-aspnet-core
            // Connection string stored in appsettings.json see http://go.microsoft.com/fwlink/?LinkId=723263
            services.AddDbContext<minicstructorContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            #endregion
            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication(); // The order of the middleware is important. Call UseAuthentication before UseAuthorization
            app.UseAuthorization(); // See https://docs.microsoft.com/en-us/aspnet/core/security/authentication/identity?view=aspnetcore-3.1&tabs=visual-studio#configure-identity-services

            // The order of middleware is important. Call UseSession after UseRouting and before UseEndpoints. 
            // See Middleware Ordering https://docs.microsoft.com/en-us/aspnet/core/fundamentals/middleware/?view=aspnetcore-5.0#order
            app.UseSession();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
