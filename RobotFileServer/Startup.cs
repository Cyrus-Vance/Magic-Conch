using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;

namespace RobotFileServer
{
    ///
    public class Startup
    {
        ///
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            FileCleaner.InitTimer();
        }

        ///
        public IConfiguration Configuration { get; }

        ///
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddSwaggerGen(c =>
            {
                c.IncludeXmlComments($"{AppDomain.CurrentDomain.BaseDirectory}/RobotFileServer.xml");

                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Cyrus QQ Robot协作API", Version = "0.02", Description = "用于Cyrus QQ机器人的相关功能的远程辅助实现" });

                /*c.AddSecurityDefinition("bearer", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "AccessToken",
                    In = ParameterLocation.Header,
                    Scheme = "bearer"
                    //Type = SecuritySchemeType.OAuth2,

                    //Flows = new OpenApiOAuthFlows
                    //{
                    //    Password = new OpenApiOAuthFlow
                    //    {
                    //        TokenUrl = new Uri("http://localhost:5066/connect/token", UriKind.Absolute),
                    //        /*AuthorizationUrl = new Uri("/connect/authorize", UriKind.Relative),
                    //        Scopes = new Dictionary<string, string>
                    //            {
                    //                { "readAccess", "Access read operations" },
                    //                { "writeAccess", "Access write operations" }
                    //            }
                    //    }
                    //}*/
            });
        }

        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "开发版本");
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
