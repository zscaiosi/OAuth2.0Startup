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
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;

namespace OAuthStartup.Extensions
{
    public static class SecurityExtension
    {
        /// <summary>
        /// Adds an arbitrary default Bearer scheme.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="config"></param>
        public static void AddDefaultJwtAuthentication(this IServiceCollection services, IConfiguration config)
        {
            DefaultJwtBearerOptions jwtOpts;

            var symmetricKey = config.GetSection("jwtSettings").GetSection("symmetricKey").Value;
            var publicKey = config.GetSection("jwtSettings").GetSection("publicKey").Value;
            // Checks signing type
            bool isSymmetric = !string.IsNullOrEmpty(symmetricKey);
            bool isAsymmetric = !string.IsNullOrEmpty(publicKey);

            if (!isSymmetric && !isAsymmetric)
                throw new ArgumentNullException(ErrorMessages.INVALID_KEY);
            
            // From appsettings.json builds options
            if (isSymmetric)
                jwtOpts = new DefaultJwtBearerOptions(
                    config.GetSection("jwtSettings").GetSection("issuers").Value.Split(','),
                    config.GetSection("jwtSettings").GetSection("audiences").Value.Split(','),
                    Encoding.ASCII.GetBytes(symmetricKey)
                );
            else
                jwtOpts = new DefaultJwtBearerOptions(
                    config.GetSection("jwtSettings").GetSection("issuers").Value.Split(','),
                    config.GetSection("jwtSettings").GetSection("audiences").Value.Split(','),
                    SecurityAlgorithms.RsaSha256Signature,
                    Encoding.ASCII.GetBytes(publicKey)
                );

            services.AddAuthentication(opt => {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultForbidScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(opt => opt = jwtOpts);
            // Authorization
            services.AddAuthorization(auth =>
            {
                auth.AddPolicy(JwtBearerDefaults.AuthenticationScheme, new AuthorizationPolicyBuilder()
                    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser().Build());
            });
        }
        /// <summary>
        /// Adds an arbitrary default Basic authentication scheme.
        /// </summary>
        /// <param name="services">MUST contain at least one BaseClientAuth<BaseRepository> derived class</param>
        /// <param name="config"></param>
        public static void AddDefaultBasicAuthentication(this IServiceCollection services, IConfiguration config)
        {
            // Needs ServiceProvider in order to access function
            IServiceProvider sp = services.BuildServiceProvider();
            var localAuth = sp.GetService<BaseClientAuth<BaseRepository>>();

            // Must provide a local way to check credentials
            if (localAuth == null)
                throw new ArgumentNullException(ErrorMessages.INVALID_AUTHENTICATION);

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