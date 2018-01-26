using BuildingBlocks.Core;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Playground.Application
{
    public class ApiInfo : IApiInfo
    {
        private ApiInfo() { }
        public string AuthenticationAuthority => null;
        public string JwtBearerAudience => null;
        public string Code => "building.blocks.playground";
        public string Title => "Building Blocks Playground";
        public string Version => "V1";
        public Assembly ApplicationAssembly => GetType().Assembly;
        public IDictionary<string, string> Scopes => null;
        public SwaggerAuthInfo SwaggerAuthInfo => null;
       
        private static readonly Lazy<ApiInfo> _instance 
            = new Lazy<ApiInfo>(() => new ApiInfo());
        public static ApiInfo Instance => _instance.Value;
    }
}
