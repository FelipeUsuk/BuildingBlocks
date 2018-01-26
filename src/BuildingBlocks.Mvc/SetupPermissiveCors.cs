using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Mvc
{
    public static class SetupPermissiveCors
    {
        public static IServiceCollection AddPermissiveCors(
            this IServiceCollection services
        ) => services
            .AddCors(options =>
            {
                options.AddPolicy("PermissiveCorsPolicy", builder => builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()
                );
            });

        public static IApplicationBuilder UsePermissiveCors(this IApplicationBuilder app)
            => app.UseCors("PermissiveCorsPolicy");
    }
}
