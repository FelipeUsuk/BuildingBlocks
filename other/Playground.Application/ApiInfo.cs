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
        public string AuthenticationAuthority => "http://localhost:5000";
        public string JwtBearerAudience => "playground";
        public string Code => "building.blocks.playground";
        public string Title => "Building Blocks Playground";
        public string Version => "V1";
        public Assembly ApplicationAssembly => GetType().Assembly;

        public IDictionary<string, string> Scopes => new Dictionary<string, string>
        {
            {"playground", Title}
        };

        public SwaggerAuthInfo SwaggerAuthInfo => new SwaggerAuthInfo("playgroundswaggerui", "", "");
       
        private static readonly Lazy<ApiInfo> _instance 
            = new Lazy<ApiInfo>(() => new ApiInfo());
        public static ApiInfo Instance => _instance.Value;
    }
}
