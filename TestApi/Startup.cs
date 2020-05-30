using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OAuthStartup;
using OAuthStartup.Enums;

namespace TestApi
{
    public class Startup : BaseStartup
    {
        public Startup(IConfiguration configuration)
        :
        base(
            configuration, (cpb) => {
            cpb.AllowAnyHeader();
            cpb.AllowAnyMethod();
            cpb.AllowAnyOrigin();
            },
            true,
            StartupEnums.JwtBearer
        )
        {

        }
        
        protected override void BeforeConfigureServices(IServiceCollection services)
        {
            // base.BeforeConfigureServices(services);
            Console.WriteLine("Startup BeforeConfigureServices");
        }

        protected override void BeforeConfigureApp(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // base.BeforeConfigureApp(app, env);
            Console.WriteLine("Startup BeforeConfigureApp");
        }
    }
}
