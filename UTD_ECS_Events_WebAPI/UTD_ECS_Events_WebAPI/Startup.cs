using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using UTD_ECS_Events_WebAPI.Repositories;
using UTD_ECS_Events_WebAPI.Services;
using Serilog;

namespace UTD_ECS_Events_WebAPI
{
    public class Startup
    {
        const string OnlyAllowHostedWebsiteEdit = "_onlyAllowHostedWebsiteEdit";
        const string AllowAllRead = "_allowAllRead";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // CORS allow all domains
            //services.AddCors(options => 
            //    options.AddPolicy("Cors",
            //        builder =>
            //        {
            //            builder.AllowAnyOrigin()
            //                .AllowAnyMethod()
            //                .AllowAnyHeader();
            //        })
            //);

            services.AddCors(options =>
            {
                options.AddPolicy(name: OnlyAllowHostedWebsiteEdit,
                                  builder =>
                                  {
                                      builder.WithOrigins("https://ecs-utdevents.web.app/");
                                  });

                options.AddPolicy(name: AllowAllRead,
                                  builder =>
                                  {
                                      builder.AllowAnyOrigin();
                                  });
            });

            services.AddMvc(options => options.EnableEndpointRouting = false);

            //    dependency injection
            services.AddTransient<IFirestoreService, FirestoreService>();
            services.AddTransient<IFirestoreRepository, FirestoreRepository>();
            services.AddTransient<IEventsService, EventsService>();
            services.AddTransient<IEventsRepository, EventsRepository>();
            services.AddTransient<IOrgsRepository, OrgsRepository>();
            services.AddTransient<IOrgsService, OrgsService>();
            services.AddTransient<ITagsRepository, TagsRepository>();
            services.AddTransient<ITagsService, TagsService>();

            //    Serilog
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console()
                .WriteTo.File("log.txt",
                    rollingInterval: RollingInterval.Day,
                    rollOnFileSizeLimit: true)
                .CreateLogger();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseCors();
            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
