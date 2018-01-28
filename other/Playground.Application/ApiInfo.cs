using BuildingBlocks.Core;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.Configuration;

namespace Playground.Application
{
    public class ApiInfo : IApiInfo
    {
        private ApiInfo(IConfiguration config)
        {
            AuthenticationAuthority = config["AuthenticationAuthority"];
        }
        public string AuthenticationAuthority { get; }
        public string JwtBearerAudience => "playground";
        public string Code => "building.blocks.playground";
        public string Title => "Building Blocks Playground";
        public string Version => "V1";
        public Assembly ApplicationAssembly => GetType().Assembly;

        public IDictionary<string, string> Scopes => new Dictionary<string, string>
        {
            {"playground", Title}
        };

        public SwaggerAuthInfo SwaggerAuthInfo => new SwaggerAuthInfo(
            "playgroundswaggerui", "", ""
            );

        public static IApiInfo Instantiate(IConfiguration config)
        {
            Instance = new ApiInfo(config);
            return Instance;
        }
        public static IApiInfo Instance { get; private set; }
   }
}
