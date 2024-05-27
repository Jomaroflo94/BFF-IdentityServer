using Duende.IdentityServer;
using Duende.IdentityServer.Models;

namespace IDP.WebApp;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
    [
        new IdentityResources.OpenId(),
        new IdentityResources.Profile(),
    ];

    public static IEnumerable<ApiResource> ApiResources { get; } =
    [
        new()
        {
            Name = "Resource.Api",
            Scopes =
            [
                "resource.api.read",
                "resource.api.write",
                "resource.api.update",
                "resource.api.delete",
                "resource.api.all"
            ]
        }
    ];

    public static IEnumerable<ApiScope> ApiScopes =>
    [
        new("resource.api.read"),
        new("resource.api.write"),
        new("resource.api.update"),
        new("resource.api.delete"),
        new("resource.api.all"),
    ];

    public static IEnumerable<Client> Clients =>
    [
        new Client
        {
            ClientId = GetVariable("RESOURCE_CLIENT_ID"),
            ClientSecrets = { new Secret(GetVariable("RESOURCE_CLIENT_SECRET").Sha256()) },
            AllowedGrantTypes = GrantTypes.ClientCredentials,
            RedirectUris =
            {
                GetVariable("RESOURCE_BASE_ADDRESS") + "/swagger/oauth2-redirect.html"
            },
            AllowedScopes =
            {
                IdentityServerConstants.StandardScopes.OpenId,
                IdentityServerConstants.StandardScopes.Profile,
                "resource.api.read",
                "resource.api.write",
                "resource.api.update",
                "resource.api.delete",
                "resource.api.all"
            },
            AllowOfflineAccess = true,
            ClientUri = GetVariable("RESOURCE_BASE_ADDRESS") + "/swagger/index.html"
        },
        new Client
        {
            ClientId = GetVariable("BFF_CLIENT_ID"),
            ClientSecrets = { new Secret(GetVariable("BFF_CLIENT_SECRET").Sha256()) },

            AllowedGrantTypes = GrantTypes.CodeAndClientCredentials,
            
            RedirectUris =
            {
                GetVariable("BFF_BASE_ADDRESS") + "/signin-oidc",
                GetVariable("CLIENT_BASE_ADDRESS") + "/signin-oidc"
            },

            PostLogoutRedirectUris =
            {
                GetVariable("BFF_BASE_ADDRESS") + "/signout-callback-oidc",
                GetVariable("CLIENT_BASE_ADDRESS") + "/signout-callback-oidc"
            },

            AllowedScopes =
            {
                IdentityServerConstants.StandardScopes.OpenId,
                IdentityServerConstants.StandardScopes.Profile,
                "resource.api.read",
                "resource.api.write",
                "resource.api.update",
                "resource.api.delete",
                "resource.api.all"
            },
            
            AllowOfflineAccess = true
        }
    ];

    private static string GetVariable(string key)
    {
        return Environment.GetEnvironmentVariable(key) ?? string.Empty;
    }
}
