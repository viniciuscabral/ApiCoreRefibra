using ApiRefibra.Implementation;
using ApiRefibra.Interface;
using ApiRefibra.Model;
using ApiRefibra.Repo;
using ApiRefibra.Exceptions;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;
using System.Reflection;
using System.IO;

namespace ApiRefibra
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

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                
               c.SwaggerDoc("v1", new OpenApiInfo
                    {
                        Version = "v1",                        
                        Title = "API Refibra",
                        Description = "A simple API to share other world vision. Acess code on [https://github.com/viniciuscabral/ApiCoreRefibra](https://github.com/viniciuscabral/ApiCoreRefibra).",
                        TermsOfService = new Uri("https://example.com/terms"),                       
                        Contact = new OpenApiContact
                        {
                            Name = "Vinícius Cabral",
                            Email = string.Empty,
                            Url = new Uri("https://www.instagram.com/vviniciuscabral/"),
                            
                        }  
                        
                    });
                
                
                    // Set the comments path for the Swagger JSON and UI.
                    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                    c.IncludeXmlComments(xmlPath);
                    
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
               app.UseDeveloperExceptionPage();
            }
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "API Refibra V1");
                c.RoutePrefix = string.Empty;
            });

            app.UseMiddleware(typeof(ErrorHandlingMiddleware));
            app.UseCors("AllowAll");
            app.UseMvc();
        }
    }
}
