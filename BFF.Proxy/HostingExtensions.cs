using Duende.Bff;
using Duende.Bff.Yarp;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Serilog;

namespace BFF.Proxy;

internal static class HostingExtensions
{
    internal static void ConfigureHost(this ConfigureHostBuilder host)
    {
        host.UseSerilog((ctx, lc) => lc
            .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] " + 
                "{SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}")
            .Enrich.FromLogContext()
            .ReadFrom.Configuration(ctx.Configuration));
    }

    internal static void ConfigureBffServices(this IServiceCollection services)
    {
        // Register BFF services and configure the BFF middleware
        // Registra servicios necesarios para el reenvio http de YARP
        services.AddBff(options => {
            options.AntiForgeryHeaderValue = GetVariable("X_CSRF_VALUE");
        }).AddRemoteApis(); //Yarp.ReverseProxy
    }

    internal static void ConfigureSecurityServices(this IServiceCollection services)
    {
        // Configure ASP.NET Authentication
        services.AddAuthentication(options => 
        {
            options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            options.DefaultSignOutScheme = OpenIdConnectDefaults.AuthenticationScheme;
        })
        // Configure ASP.NET Cookie Authentication
        .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
        {
            options.ExpireTimeSpan = TimeSpan.FromHours(8);
            options.SlidingExpiration = false;
            options.Cookie.Name = "__ClientSPA";
            // When the value is Strict the cookie will only be sent along with "same-site" requests.
            options.Cookie.SameSite = SameSiteMode.Strict;
        })
        // Configure OpenID Connect
        .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options => 
        {
            options.Authority = GetVariable("IDP_BASE_ADDRESS");
            options.ClientId = GetVariable("BFF_CLIENT_ID");
            options.ClientSecret = GetVariable("BFF_CLIENT_SECRET");

            options.ResponseType = OpenIdConnectResponseType.Code;
            options.ResponseMode = OpenIdConnectResponseMode.Query;
            //options.UsePkce = true;

            // Go to the user info endpoint to retrieve additional claims after creating an identity from the id_token
            options.GetClaimsFromUserInfoEndpoint = true;
            // Store access and refresh tokens in the authentication cookie
            options.SaveTokens = true;

            options.RequireHttpsMetadata = true;

            options.Scope.Clear();
            options.Scope.Add(OpenIdConnectScope.OpenId);
            options.Scope.Add(OpenIdConnectScope.OpenIdProfile);
            options.Scope.Add(OpenIdConnectScope.OfflineAccess);
            options.Scope.Add("resource.api.read");

            options.ClaimActions.Remove("aud");
            options.ClaimActions.DeleteClaim("sid");
            options.ClaimActions.DeleteClaim("idp");
        });

        services.AddAuthorization();
    }

    internal static void UseBffConfiguration(this WebApplication app)
    {
        //The call to the AsBffApiEndpoint() fluent helper method
        //adds BFF support to the local APIs. This includes anti-forgery
        //protection as well as suppressing login redirects on authentication failures and
        //instead returning 401 and 403 status codes under the appropriate circumstances.
        app.MapControllers().AsBffApiEndpoint();

        // Adds the BFF management endpoints (/bff/login, /bff/logout, ...)
        app.MapBffManagementEndpoints();

        // proxy any call to local /remote to the actual remote api
        // passing the access token
        app.MapRemoteBffApiEndpoint(
                "/api/ProgrammingLanguage", GetVariable("RESOURCE_BASE_ADDRESS") + "/api/ProgrammingLanguage")
            .RequireAccessToken(TokenType.User);
    }

    private static string GetVariable(string key)
    {
        return Environment.GetEnvironmentVariable(key)?? string.Empty;
    }
}