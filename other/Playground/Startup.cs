using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BuildingBlocks.Autofac;
using BuildingBlocks.Core;
using BuildingBlocks.Idempotency;
using BuildingBlocks.Mediatr.Autofac;
using BuildingBlocks.Mediatr.Exceptions;
using BuildingBlocks.Swagger;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
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
                .AddCustomSwagger(ApiInfo.Instance);

            return services.ConvertToAutofac(
                MediatrModule.Create(ApiInfo.Instance.ApplicationAssembly)
                );
        }
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseMvcWithDefaultRoute();
            app.UseCustomSwagger(ApiInfo.Instance);
            app.UseApplication();
        }
    }
}
