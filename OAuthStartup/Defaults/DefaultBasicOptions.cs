using System;
using Microsoft.AspNetCore.Authentication;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace OAuthStartup.Defaults
{
    public class DefaultBasicOptions : AuthenticationSchemeOptions
    {
        /// <summary>
        /// The function responsible for authenticating with a remote Authentication Server
        /// </summary>
        /// <value></value>
        public Func<string, string, Task<HttpResponseMessage>> RemoteAuthenticationAsync {get;set;} = null;
        /// <summary>
        /// Remote server's URL
        /// </summary>
        /// <value></value>
        public string RemoteUrl {get;set;}
        /// <summary>
        /// Authenticates locally passing credentials
        /// </summary>
        /// <value></value>
        public Func<string, string, Task<bool>> LocalAuthenticationAsync {get;set;} = null;
    }
}