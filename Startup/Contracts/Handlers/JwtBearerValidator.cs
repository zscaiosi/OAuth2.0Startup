using System;

namespace Startup.Contracts.Handlers
{
    public class JwtBearerValidator<T>
    {
        public Action<T> Handler {get;set;}
    }
}