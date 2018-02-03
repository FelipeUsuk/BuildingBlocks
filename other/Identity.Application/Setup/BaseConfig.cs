using System.Collections.Generic;
using System.Threading.Tasks;
using BuildingBlocks.IdentityServer4.RavenDB.Stores;
using IdentityServer4;
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
            },

            new Client
            {
                ClientId = "playgroundweb",
                ClientName = "PlaygroundWeb",
                ClientSecrets = new List<Secret>
                {
                    new Secret("secret".Sha256())
                },
                ClientUri = $"{clientsUrl["PlaygroundWeb"]}",                             // public uri of the client
                AllowedGrantTypes = GrantTypes.Hybrid,
                AllowAccessTokensViaBrowser = false,
                RequireConsent = false,
                AllowOfflineAccess = true,
                AlwaysIncludeUserClaimsInIdToken = true,
                RedirectUris = new List<string>
                {
                    $"{clientsUrl["PlaygroundWeb"]}/signin-oidc"
                },
                PostLogoutRedirectUris = new List<string>
                {
                    $"{clientsUrl["PlaygroundWeb"]}/signout-callback-oidc"
                },
                AllowedScopes = new List<string>
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    IdentityServerConstants.StandardScopes.OfflineAccess,
                    "playground",
                }
                
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
                    {"PlaygroundApi", configuration["PlaygroundApi"]},
                    {"PlaygroundWeb", configuration["PlaygroundWeb"]}
                };

                foreach (var client in GetClients(urls))
                {
                    await clientStore.StoreAsync(client);
                }
            }
        }

    }
}
