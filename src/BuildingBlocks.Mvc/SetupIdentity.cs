using System.IdentityModel.Tokens.Jwt;
using BuildingBlocks.Core;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Mvc
{
    public static class SetupIdentity
    {
        public static IServiceCollection AddCustomIdentity(
            this IServiceCollection services, 
            IApiInfo apiInfo
            )
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(options =>
            {
                options.Authority = apiInfo.AuthenticationAuthority;
                options.RequireHttpsMetadata = false;
                options.Audience = apiInfo.JwtBearerAudience;
            });

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IUser, HttpContextUser>();

            return services;
        }
    }
}
