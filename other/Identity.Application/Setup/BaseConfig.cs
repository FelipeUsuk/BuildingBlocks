using System.Collections.Generic;
using System.Threading.Tasks;
using BuildingBlocks.IdentityServer4.RavenDB.Stores;
using IdentityServer4.Models;
using Microsoft.Extensions.Configuration;

namespace Identity.Application.Setup
{
    public static class BaseConfig
    {
        public static IEnumerable<ApiResource> GetApis() => new[]
        {
            new ApiResource("playground", "PlaygroundService")
        };

        public static IEnumerable<IdentityResource> GetResources() => new IdentityResource[]
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile()
        };

        public static IEnumerable<Client> GetClients(
            IDictionary<string, string> clientsUrl
        ) => new[]
        {
            new Client
            {
                ClientId = "playgroundswaggerui",
                ClientName = "Playground Swagger UI",
                AllowedGrantTypes = GrantTypes.Implicit,
                AllowAccessTokensViaBrowser = true,

                RedirectUris = { $"{clientsUrl["PlaygroundApi"]}/swagger/o2c.html" },
                PostLogoutRedirectUris = { $"{clientsUrl["PlaygroundApi"]}/swagger/" },

                AllowedScopes =
                {
                    "playground"
                },
                RequireConsent = false
            } 
        };

        public static async Task LoadSeed(
            IRavenDBClientStore clientStore, 
            IRavenDBResourceStore resourceStore,
            IConfiguration configuration
            )
        {
            if (!await resourceStore.HasStoredApis())
            {
                foreach (var api in GetApis())
                {
                    await resourceStore.StoreAsync(api);
                }
            }

            if (!await resourceStore.HasStoredIdentities())
            {
                foreach (var identity in GetResources())
                {
                    await resourceStore.StoreAsync(identity);
                }
            }

            if (!await clientStore.HasStoredClients())
            {
                var urls = new Dictionary<string, string>
                {
                    {"PlaygroundApi", configuration["PlaygroundApi"]}
                };

                foreach (var client in GetClients(urls))
                {
                    await clientStore.StoreAsync(client);
                }
            }
        }

    }
}
