using System;

namespace OAuthStartupBase.Contracts.Handlers
{
    public class JwtBearerValidator<T>
    {
        public Action<T> Handler {get;set;}
    }
}