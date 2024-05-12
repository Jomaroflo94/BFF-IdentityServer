using Duende.IdentityServer;
using Duende.IdentityServer.Models;

namespace ISP.WebApp;

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
            ClientId = "Swagger.Resource.Api",
            ClientSecrets = { new Secret("Secret.Swagger.Resource.Api".Sha256()) },
            AllowedGrantTypes = GrantTypes.ClientCredentials,
            RedirectUris =
            {
                "https://localhost:7293/swagger/oauth2-redirect.html"
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
            ClientUri = "https://localhost:7293/swagger/index.html"
        },
        new Client
        {
            ClientId = "BFF.Proxy",
            ClientSecrets = { new Secret("Secret.BFF.Proxy".Sha256()) },

            AllowedGrantTypes = GrantTypes.Code,
            
            // where to redirect to after login
            RedirectUris =
            {
                "https://localhost:7291/signin-oidc"
            },

            // where to redirect to after logout
            PostLogoutRedirectUris =
            {
                "https://localhost:7291/signout-callback-oidc"
            },

            AllowedScopes =
            {
                IdentityServerConstants.StandardScopes.OpenId,
                IdentityServerConstants.StandardScopes.Profile
            },
            
            AllowOfflineAccess = true
        }
    ];
}
