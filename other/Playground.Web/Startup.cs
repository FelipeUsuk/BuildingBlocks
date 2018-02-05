using System;
using System.Linq;
using System.Net.Http;
using BuindingBlocks.Resilience.Http;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.HealthChecks;
using Microsoft.Extensions.Logging;
using Playground.Web.Services;
using Polly;

namespace Playground.Web
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddFeatureFoldersSupport();

            services.AddHealthChecks(checks =>
                {
                    checks.AddUrlCheck($"{_configuration["AuthenticationAuthority"]}/hc");
                    checks.AddUrlCheck($"{_configuration["PlaygroundService"]}/hc");
                });

            services.AddAuthentication(options => {
                    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
                })
                .AddCookie()
                .AddOpenIdConnect(options => {
                    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.Authority = _configuration["AuthenticationAuthority"];
                    options.ClientId = "playgroundweb";
                    options.ClientSecret = "secret";
                    options.ResponseType = "code id_token";
                    options.SaveTokens = true;
                    options.GetClaimsFromUserInfoEndpoint = true;
                    options.RequireHttpsMetadata = false;
                    options.Scope.Add("offline_access");
                    options.Scope.Add("playground");
                });

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            //services.AddSingleton<IHttpClient, StandardHttpClient>();
            services.AddSingleton(sp => (PolicyFactory)((origin) =>
            {
                var logger = sp.GetRequiredService<ILogger<ResilientHttpClient>>();
                return new Policy[]
                {
                    Policy.Handle<HttpRequestException>()
                        .WaitAndRetryAsync(
                            // number of retries
                            6,
                            // exponential backofff
                            retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                            // on retry
                            (exception, timeSpan, retryCount, context) =>
                            {
                                var msg = $"Retry {retryCount} implemented with Polly's RetryPolicy " +
                                          $"of {context.PolicyKey} " +
                                          $"at {context.ExecutionKey}, " +
                                          $"due to: {exception}.";
                                logger.LogWarning(msg);
                                logger.LogDebug(msg);
                            }),
                    Policy.Handle<HttpRequestException>()
                        .CircuitBreakerAsync( 
                            // number of exceptions before breaking circuit
                            6,
                            // time circuit opened before retry
                            TimeSpan.FromMinutes(1),
                            (exception, duration) =>
                            {
                                // on circuit opened
                                logger.LogTrace("Circuit breaker opened");
                            },
                            () =>
                            {
                                // on circuit closed
                                logger.LogTrace("Circuit breaker reset");
                            })
                };
            }));
            services.AddSingleton<IHttpClient, ResilientHttpClient>();
            services.AddScoped<IEchoService, EchoService>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseMvcWithDefaultRoute();
        }
    }
}
