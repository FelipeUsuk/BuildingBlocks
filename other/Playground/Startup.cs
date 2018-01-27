using System;
using BuildingBlocks.Autofac;
using BuildingBlocks.Idempotency;
using BuildingBlocks.Mediatr.Autofac;
using BuildingBlocks.Mediatr.Exceptions;
using BuildingBlocks.Swagger;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Playground.Application;

namespace Playground
{
    public class Startup
    {
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services
                .AddMvc(options =>
                {
                    options.Filters.Add(typeof(HttpGlobalExceptionFilter));
                });

            services
                .AddApplication<InMemoryRequestManager>()
                .AddCustomIdentity(ApiInfo.Instance)
                .AddPermissiveCors()
                .AddCustomSwagger(ApiInfo.Instance);

            return services.ConvertToAutofac(
                MediatrModule.Create(ApiInfo.Instance.ApplicationAssembly)
                );
        }
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseApplication();
            app.UsePermissiveCors();
            app.UseCustomSwagger(ApiInfo.Instance);
            app.UseAuthentication();
            app.UseMvcWithDefaultRoute();
        }
    }
}
