using System;
using System.Collections.Generic;

namespace Startup.Contracts.Policies
{
    public class CorsPolicy
    {
        public string Name {get;set;}
        public string[] Headers {get;set;}
        public string[] Methods {get;set;}
        public string[] Origins {get;set;}
    }
}