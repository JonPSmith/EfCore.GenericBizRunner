using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutoMapper;
using DataLayer.EfCode;
using EfCoreInAction.Logger;
using GenericBizRunner.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ServiceLayer;

namespace EfCoreInAction
{
    public class Startup
    {

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
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

            //Now I use AutoFac to do some of the more complex registering of services
            var containerBuilder = new ContainerBuilder();

            #region GenericBizRunner parts
            // Need to call AddAutoMapper to set up the mappings any GenericAction From/To Biz Dtos
            services.AddAutoMapper(); 
            //GenericBizRunner has two AutoFac modules that can register all the services needed
            //This one is the simplest, as it sets up the link to the application's DbContext
            containerBuilder.RegisterModule(new BizRunnerDiModule<EfCoreContext>());
            //Now I use the ServiceLayer AutoFac module that registers all the other DI items, such as my biz logic
            containerBuilder.RegisterModule(new ServiceLayerModule());
            #endregion

            containerBuilder.Populate(services);
            var container = containerBuilder.Build();
            return new AutofacServiceProvider(container); 
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
