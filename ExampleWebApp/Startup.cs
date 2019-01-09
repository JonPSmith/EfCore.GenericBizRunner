using System.Reflection;
using BizDbAccess.Orders.Concrete;
using BizLogic.Orders.Concrete;
using DataLayer.EfCode;
using EfCoreInAction.Logger;
using GenericBizRunner.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NetCore.AutoRegisterDi;
using ServiceLayer.BookServices;
using ServiceLayer.OrderServices;

namespace EfCoreInAction
{
    public class Startup
    {

        public Startup(Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public Microsoft.Extensions.Configuration.IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();


            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            //--------------------------------------------------------------------
            //var connection = Configuration.GetConnectionString("DefaultConnection");
            //Swapped over to sqlite in-memory database
            var connectionStringBuilder = new SqliteConnectionStringBuilder { DataSource = ":memory:" };
            var connectionString = connectionStringBuilder.ToString();
            var connection = new SqliteConnection(connectionString);
            connection.Open();  //see https://github.com/aspnet/EntityFramework/issues/6968
            services.AddDbContext<EfCoreContext>(options => options.UseSqlite(connection));
            //--------------------------------------------------------------------

            #region GenericBizRunner parts
            //This sets up the GenericBizRunner to use one DbContext. Note: you could add a GenericBizRunnerConfig here if you needed it
            services.RegisterGenericBizRunnerBasic<EfCoreContext>(Assembly.GetAssembly(typeof(WebChangeDeliveryDto)));
            //This sets up the GenericBizRunner to work with multiple DbContext
            //see https://github.com/JonPSmith/EfCore.GenericBizRunner/wiki/Using-multiple-DbContexts
            //services.RegisterGenericBizRunnerMultiDbContext(Assembly.GetAssembly(typeof(WebChangeDeliveryDto)));
            #endregion

            //now we register the public classes with public interfaces in the three layers
            services.RegisterAssemblyPublicNonGenericClasses(
                Assembly.GetAssembly(typeof(BookListDto)), //Service layer
                Assembly.GetAssembly(typeof(PlaceOrderAction)), //BizLogic
                Assembly.GetAssembly(typeof(PlaceOrderDbAccess)) //BizDbAccess
            ).AsPublicImplementedInterfaces();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, 
            ILoggerFactory loggerFactory, IHttpContextAccessor httpContextAccessor)
        {
            //Remove the standard loggers because they slow the applictaion down
            //loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            //loggerFactory.AddDebug();
            loggerFactory.AddProvider(new RequestTransientLogger(() => httpContextAccessor));
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
