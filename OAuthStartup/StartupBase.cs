using System;
using OAuthStartup.Enums;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OAuthStartup.Extensions;
using Microsoft.AspNetCore.Cors.Infrastructure;

namespace OAuthStartup
{
    public abstract class BaseStartup
    {
        protected IConfiguration Configuration {get;set;}
        protected Action<CorsPolicyBuilder> CorsPolicyAction {get;set;}
        protected StartupEnums AuthType {get;set;}
        protected bool UseDefaultSecurity {get;set;}
        public BaseStartup(IConfiguration configuration, Action<CorsPolicyBuilder> corsPolicy, bool useDefaultSecurity = true, StartupEnums authType = StartupEnums.JwtBearer)
        {
            Configuration = configuration;
            CorsPolicyAction = corsPolicy;
            UseDefaultSecurity = useDefaultSecurity;
            AuthType = authType;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        protected virtual void BeforeConfigureServices(IServiceCollection services)
        {
            
        }
        /// <summary>
        /// This method injects dependencies and adds services to the application.
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            // Custom services
            this.BeforeConfigureServices(services);
            // Default services an API needs to have
            services.AddCors();
            services.AddSingleton<IConfiguration>(Configuration);

            if (AuthType == StartupEnums.JwtBearer)
                services.AddDefaultJwtAuthentication(Configuration);
            else
                services.AddDefaultBasicAuthentication(Configuration);

            services.AddControllers();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        protected virtual void BeforeConfigureApp(IApplicationBuilder app, IWebHostEnvironment env)
        {
            
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Custom app pipeline
            this.BeforeConfigureApp(app, env);
            // Custom CORS policy
            app.UseCors(CorsPolicyAction);

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            // More middlewares after controllers
            this.AfterConfigureApp(app, env);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        protected virtual void AfterConfigureApp(IApplicationBuilder app, IWebHostEnvironment env)
        {

        }
    }
}
