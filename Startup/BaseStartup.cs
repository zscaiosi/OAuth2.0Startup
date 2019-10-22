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
        private readonly List<CorsPolicy> _corsPolicies;
        private readonly EStartupType _type;
        private readonly List<EConfigs> _eConfigs;

        public BaseStartup(IConfiguration configuration, EStartupType type, List<EConfigs> eConfigs, List<CorsPolicy> corsPolicies) {
            _configs = configuration;
            _type = type;
            _eConfigs = eConfigs;
            _corsPolicies = corsPolicies;
        }

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
                        context.HttpContext
                        return Task.CompletedTask;
                    },
                    OnAuthenticationFailed = context =>
                    {
                        Console.WriteLine("Token inválido..:. " + context.Exception.Message);
                        return Task.CompletedTask;
                    },
                    OnTokenValidated = context =>
                    {
                        Console.WriteLine("Token válido...: " + context.SecurityToken);
                        return Task.CompletedTask;
                    }
                };
            });
        }

        protected virtual void BeforeConfigureServices(IServiceCollection services) {
            
        }

        public virtual void BeforeConfigureApp(IApplicationBuilder app, IWebHostEnvironment env) {
            
        }
        public void ConfigureServices(IServiceCollection services) {
            BeforeConfigureServices(services);
            // CORS Policy
            services.AddCors(options =>
            {
                foreach (var p in _corsPolicies) {
                    options.AddPolicy(p.Name,
                    builder =>
                    {
                        builder
                        .WithHeaders(p.Headers)
                        .WithMethods(p.Methods)
                        .WithOrigins(p.Origins);
                    });
                }
            });
            // JWT Bearer Token
            AddBearerAuthentication(ref services);
        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            BeforeConfigureApp(app, env);
        }
    }
}
