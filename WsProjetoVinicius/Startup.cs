﻿using ApiJenaFusekiRefibra.Implementation;
using ApiJenaFusekiRefibra.Interface;
using ApiJenaFusekiRefibra.Model;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WsProjetoVinicius.Model;
using WsTestes.Repo;

namespace WsTestes
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddMvc();
          
            services.AddScoped<IFusekiServices, FusekiServices>();
            
            services.AddDbContext<ApplicationDbContext>(context => { context.UseInMemoryDatabase("ProjetoVini"); });
            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));
            services.AddAutoMapper();
            services.AddCors(options => 
            {
                options.AddPolicy("AllowAll",
                    builder => 
                    {
                        builder.AllowAnyOrigin()
                        .AllowCredentials()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                    });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            //if (env.IsDevelopment())
            //{
            //    app.UseDeveloperExceptionPage();
            //}
            app.UseMiddleware(typeof(ErrorHandlingMiddleware));
            app.UseCors("AllowAll");
            app.UseMvc();
        }
    }
}
