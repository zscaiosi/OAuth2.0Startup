using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace OAuthStartup.Defaults
{
    public class DefaultJwtBearerOptions : JwtBearerOptions
    {
        public DefaultJwtBearerOptions(IEnumerable<string> issuers, IEnumerable<string> audiences, string alg, byte[] key)
        {
            throw new NotImplementedException();
            // var rsa = RSA.Create();

            // this.TokenValidationParameters = new TokenValidationParameters
            // {
            //     ValidateIssuer = true,
            //     ValidateAudience = true,
            //     ValidateLifetime = true,
            //     ValidateIssuerSigningKey = true,
            //     // Now the validations
            //     ValidIssuers = issuers,
            //     ValidAudiences = audiences,
            //     IssuerSigningKey = new RsaSecurityKey().Rsa
            // };
            // // Logs events
            // this.Events = new JwtBearerEvents
            // {
            //     OnMessageReceived = (context) =>
            //     {
            //         Console.WriteLine($"JWT OnMessageReceived: {context.Token}");
            //         return Task.CompletedTask;
            //     },
            //     OnChallenge = (context) =>
            //     {
            //         Console.WriteLine($"JWT OnChallenge: {context.ErrorDescription}");
            //         return Task.CompletedTask;
            //     },
            //     OnAuthenticationFailed = (context) =>
            //     {
            //         Console.WriteLine($"JWT OnAuthenticationFailed: {context.Exception.Message}");
            //         return Task.CompletedTask;
            //     },
            //     OnTokenValidated = (context) =>
            //     {
            //         var sbClaims = new StringBuilder();
            //         foreach (var c in context.Principal.Claims)
            //         {
            //             if (!string.IsNullOrEmpty(c.Value))
            //                 sbClaims.Append($"{c.Type}; ");
            //         }
            //         Console.WriteLine($"JWT OnTokenValidated: {sbClaims.ToString()}");
            //         return Task.CompletedTask;
            //     }
            // };
        }
        public DefaultJwtBearerOptions(IEnumerable<string> issuers, IEnumerable<string> audiences, byte[] symmetricKey)
        {
            this.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                // Now the validations
                ValidIssuers = issuers,
                ValidAudiences = audiences,
                IssuerSigningKey = new SymmetricSecurityKey(symmetricKey)
            };
            // Logs events
            this.Events = new JwtBearerEvents
            {
                OnMessageReceived = (context) =>
                {
                    Console.WriteLine($"JWT OnMessageReceived: {context.Token}");
                    return Task.CompletedTask;
                },
                OnChallenge = (context) =>
                {
                    Console.WriteLine($"JWT OnChallenge: {context.ErrorDescription}");
                    return Task.CompletedTask;
                },
                OnAuthenticationFailed = (context) =>
                {
                    Console.WriteLine($"JWT OnAuthenticationFailed: {context.Exception.Message}");
                    return Task.CompletedTask;
                },
                OnTokenValidated = (context) =>
                {
                    var sbClaims = new StringBuilder();
                    foreach (var c in context.Principal.Claims)
                    {
                        if (!string.IsNullOrEmpty(c.Value))
                            sbClaims.Append($"{c.Type}; ");
                    }
                    Console.WriteLine($"JWT OnTokenValidated: {sbClaims.ToString()}");
                    return Task.CompletedTask;
                }
            };
        }
    }
}