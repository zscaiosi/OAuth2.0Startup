using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using OAuthStartup.Constants;
using OAuthStartup.Defaults;
using System.Text;
using System;
using System.Security.Claims;

namespace OAuthStartup.AuthHandlers
{
    public class BasicAuthenticationHandler : AuthenticationHandler<DefaultBasicOptions>
    {
        public BasicAuthenticationHandler(IOptionsMonitor<DefaultBasicOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
        :
        base(options, logger, encoder, clock)
        {

        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns>Task<AuthenticateResult></returns>
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            // Checks Authorization header
            string authHeader = (string)Request.Headers["Authorization"] ?? string.Empty;

            if (string.IsNullOrEmpty(authHeader) || !authHeader.Contains("Basic"))
                return AuthenticateResult.Fail(ErrorMessages.UNAUTHORIZED);
            if (authHeader.Replace("Basic ", "").Length % 4 != 0)
                return AuthenticateResult.Fail(ErrorMessages.UNAUTHORIZED);
            // If there is a custom authentication process, then uses it
            if (Options.RemoteAuthenticationAsync != null)
            {
                var result = await Options.RemoteAuthenticationAsync(Options.RemoteUrl, authHeader);
                if (!result.IsSuccessStatusCode)
                    return AuthenticateResult.Fail(ErrorMessages.UNAUTHORIZED);
            }
            else if (Options.LocalAuthenticationAsync != null)
            {
                var (clientId, clientSecret) = (DecodeCredentials(authHeader)[0], DecodeCredentials(authHeader)[1]);

                var result = await Options.LocalAuthenticationAsync(clientId, clientSecret);
                if (!result)
                    return AuthenticateResult.Fail(ErrorMessages.UNAUTHORIZED);
            }
            else
            {
                throw new ArgumentNullException(ErrorMessages.INVALID_AUTHENTICATION);
            }
            
            return WriteTokenToClaims(authHeader);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="base64"></param>
        /// <returns>AuthenticateResult</returns>
        private AuthenticateResult WriteTokenToClaims(string base64)
        {
            var claims = new Claim[]
            {
                new Claim(DefaultConsts.CLIENT_ID, DecodeCredentials(base64)[0])
            };

            var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, Scheme.Name));

            return AuthenticateResult.Success(new AuthenticationTicket(principal, Scheme.Name));
        }
        /// <summary>
        /// Decodes from base64 to clientId and clientSecret
        /// </summary>
        /// <param name="base64"></param>
        /// <returns></returns>
        private string[] DecodeCredentials(string base64) =>
            Encoding.ASCII.GetString(Convert.FromBase64String(base64)).Split(':');
    }
}