using BuildingBlocks.Core;
using BuildingBlocks.Idempotency;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Playground.Application
{
    public static class Startup
    {
        public static IServiceCollection AddApplication<TRequestManager>(
            this IServiceCollection services
        ) where TRequestManager : class, IRequestManager
        {
            return services
                .AddSingleton<IRequestManager, TRequestManager>()
                .AddSingleton<IApiInfo>(ApiInfo.Instance);
        }

        public static IApplicationBuilder UseApplication(
            this IApplicationBuilder app
            )
        {
            return app;
        }
    }
}
