using IDP.WebApp.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace IDP.WebApp;

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

    internal static void ConfigureDbContextServices(this IServiceCollection services)
    {
        services.AddDbContext<ApplicationDbContext>(options => {
            options.UseSqlite("Data Source=IDP.db;");
        });
    }

    internal static void ConfigureIdentityServices(this IServiceCollection services)
    {
        services.AddIdentity<IdentityUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        services.AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;

                // see https://docs.duendesoftware.com/identityserver/v6/fundamentals/resources/
                options.EmitStaticAudienceClaim = true;
            })
            .AddInMemoryIdentityResources(Config.IdentityResources)
            .AddInMemoryApiResources(Config.ApiResources)
            .AddInMemoryApiScopes(Config.ApiScopes)
            .AddInMemoryClients(Config.Clients)
            .AddAspNetIdentity<IdentityUser>();
    }

    internal static void ConfigureSecurityServices(this IServiceCollection services)
    {
        services.AddAuthentication();
            // .AddGoogle(options =>
            // {
            //     options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;

            //     // register your IdentityServer with Google at https://console.developers.google.com
            //     // enable the Google+ API
            //     // set the redirect URI to https://localhost:7292/signin-google
            //     options.ClientId = "copy client ID from Google here";
            //     options.ClientSecret = "copy client secret from Google here";
            // });

        // builder.Services.AddCors(options =>
        // {
        //     options.AddPolicy(name: "SecurityCORS", policy => {
        //         policy.WithOrigins(Environment.GetEnvironmentVariable("BFF_AUTHORITY") ?? string.Empty)
        //             .AllowCredentials()
        //             .WithMethods("GET", "POST", "PUT", "DELETE");
        //     });
        // });
    }

    internal static void UseSecurityConfiguration(this WebApplication app)
    {
        //app.UseCors("SecurityCORS");
        app.UseCors(options => 
        {
            options.WithOrigins(GetVariable("RESOURCE_BASE_ADDRESS").Split(';'))
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
        app.UseIdentityServer();
        app.UseAuthorization();
    }

    private static string GetVariable(string key)
    {
        return Environment.GetEnvironmentVariable(key)?? string.Empty;
    }
}