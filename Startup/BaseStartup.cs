using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Startup.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using System.Collections.Generic;
using Startup.Contracts.Policies;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Threading.Tasks;
using System;
using Startup.Extensions;

namespace Startup
{
    public abstract class BaseStartup
    {
        public IConfiguration _configs {get;}
        private readonly CorsPolicy _corsPolicy;
        private readonly EStartupType _type;
        private readonly List<EConfigs> _eConfigs;

        public BaseStartup(IConfiguration configuration, EStartupType type, List<EConfigs> eConfigs, CorsPolicy corsPolicy) {
            _configs = configuration;
            _type = type;
            _eConfigs = eConfigs;
            _corsPolicy = corsPolicy;
        }
        /// <summary>Adds JWT Authentication</summary>
        /// <param name="services">The main IServiceCollection's reference</param>
        private void AddBearerAuthentication(ref IServiceCollection services) {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(bearerOpt => {
                bearerOpt.TokenValidationParameters = new TokenValidationParameters
                {
                    RequireAudience = true,
                    RequireExpirationTime = true,
                    RequireSignedTokens = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidIssuer = _configs[_eConfigs.Find(e => e == EConfigs.Issuer).ToString()],
                    ValidAudiences = _configs[_eConfigs.Find(e => e == EConfigs.Audiences).ToString()].ConfigStringToCollection(),
                    ValidateIssuerSigningKey = _eConfigs.Contains(EConfigs.SecurityKey),
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(
                            _configs[_eConfigs.Find(e => e == EConfigs.SecurityKey).ToString()]
                        )
                    )
                };
                // Saves token for later use
                bearerOpt.SaveToken = true;
                // Define the events handler
                bearerOpt.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        Console.WriteLine("Client Address Family:");
                        Console.WriteLine(context.HttpContext.Connection.RemoteIpAddress.AddressFamily);
                        Console.WriteLine("Client Address IsIPv4MappedToIPv6:");
                        Console.WriteLine(context.HttpContext.Connection.RemoteIpAddress.IsIPv4MappedToIPv6);
                        Console.WriteLine("Client Port:");
                        Console.WriteLine(context.HttpContext.Connection.RemotePort);

                        return Task.CompletedTask;
                    },
                    OnAuthenticationFailed = context =>
                    {
                        Console.WriteLine("Token inválido... " + context.Exception.Message);
                        // If has redirect url, then applies else sets status code
                        var redirect = _configs["UnauthorizedRedirect"];

                        if (!string.IsNullOrEmpty(redirect))
                            context.Response.Redirect(redirect);
                        else
                            context.Response.StatusCode = 401;

                        return Task.CompletedTask;
                    },
                    OnTokenValidated = context =>
                    {
                        Console.WriteLine("Token válido... " + context.SecurityToken);
                        return Task.CompletedTask;
                    }
                };
            });
        }
        /// <summary>Called at top of ConfigureServices</summary>
        /// <param name="services">The main IServiceCollection's reference</param>
        protected virtual void BeforeConfigureServices(ref IServiceCollection services) {
            
        }
        /// <summary>Called at top of Configure</summary>
        /// <param name="services">The main IServiceCollection's reference</param>
        public virtual void BeforeConfigureApp(ref IApplicationBuilder app, ref IWebHostEnvironment env) {
            
        }
        public void ConfigureServices(IServiceCollection services) {
            BeforeConfigureServices(ref services);
            // CORS Policy
            services.AddCors(options =>
            {
                options.AddPolicy(_corsPolicy.Name,
                builder =>
                {
                    builder
                    .WithHeaders(_corsPolicy.Headers)
                    .WithMethods(_corsPolicy.Methods)
                    .WithOrigins(_corsPolicy.Origins);
                });
            });
            // JWT Bearer Token
            AddBearerAuthentication(ref services);
        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            BeforeConfigureApp(ref app, ref env);

            app.UseCors(_corsPolicy.Name);
            app.UseAuthorization();
            app.UseMvc();
        }
    }
}
