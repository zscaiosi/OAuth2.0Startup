using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Startup.Enums;
using Microsoft.Extensions.Configuration;

namespace Startup
{
    public class BaseStartup
    {
        private IConfiguration Configs {get;set;}

        protected BaseStartup(EStartupType type) {
            
        }
    }
}
