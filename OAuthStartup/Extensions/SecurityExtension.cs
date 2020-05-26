using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System;
using Microsoft.Extensions.Configuration;
using OAuthStartup.Defaults;
using OAuthStartup.Constants;
using System.Text;
using OAuthStartup.AuthHandlers;
using OAuthStartup.BaseClasses;
using System.Threading.Tasks;

namespace OAuthStartup.Extensions
{
    public static class SecurityExtension
    {
        public static void AddDefaultJwtAuthentication(this IServiceCollection services, IConfiguration config)
        {
            var symmetricKey = config.GetSection("jwtSettings").GetSection("symmetricKey").Value;
            if (string.IsNullOrEmpty(symmetricKey))
                throw new ArgumentNullException(ErrorMessages.INVALID_SYMMETRIC_KEY);
            
            // From appsettings.json
            var jwtOpts = new DefaultJwtBearerOptions(
                config.GetSection("jwtSettings").GetSection("issuers").Value.Split(','),
                config.GetSection("jwtSettings").GetSection("audiences").Value.Split(','),
                Encoding.ASCII.GetBytes(symmetricKey)
            );

            services.AddAuthentication(opt => {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultForbidScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(opt => opt = jwtOpts);
        }
        public static void AddDefaultBasicAuthentication(this IServiceCollection services, IConfiguration config)
        {
            // Needs ServiceProvider in order to access function
            IServiceProvider sp = services.BuildServiceProvider();
            var localAuth = sp.GetService<BaseClientAuth<BaseRepository>>();

            services.AddAuthentication(opt => {
                opt.DefaultAuthenticateScheme = DefaultConsts.BASIC_AUTH_SCHEME;
                opt.DefaultChallengeScheme = DefaultConsts.BASIC_AUTH_SCHEME;
            })
            .AddScheme<DefaultBasicOptions, BasicAuthenticationHandler>(DefaultConsts.BASIC_AUTH_SCHEME, (opt) => {
                opt = new DefaultBasicOptions
                {
                    RemoteUrl = string.Empty,
                    RemoteAuthenticationAsync = null,
                    LocalAuthenticationAsync = localAuth.CheckCredentials
                };
            });
        }
    }
}