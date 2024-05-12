using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Resource.Api.DbContexts;
using Resource.Api.Repositories;
using Serilog;

namespace Resource.Api;

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

    internal static void ConfigureSwaggerServices(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>  {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "Resource Api"
            });

            options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows
                {
                    ClientCredentials = new OpenApiOAuthFlow
                    {
                        TokenUrl = new Uri($"{GetVariable("AUTHORITY")}/connect/token"),
                        Scopes = new Dictionary<string, string>() {
                            { "resource.api.read", "Read Access" },
                            { "resource.api.write", "Write Access" },
                            { "resource.api.update", "Update Access" },
                            { "resource.api.delete", "Delete Access" },
                            { "resource.api.all", "Full Access" }
                        }
                    }
                }
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement() 
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "oauth2"
                        },
                        Scheme = "oauth2",
                        Name = "oauth2",
                        In = ParameterLocation.Header
                    },
                    new List<string>()
                }
            });
        }); 
    }

    internal static void ConfigureDbContextServices(this IServiceCollection services)
    {
        services.AddDbContext<ApiContext>(options => {
            options.UseInMemoryDatabase(databaseName: "ResourceDb");
        });
    }

    internal static void ConfigureRepositoriesServices(this IServiceCollection services)
    {
        services.AddScoped<IProgrammingLanguagesRepository, 
            ProgrammingLanguagesRepository>();
    }

    internal static void ConfigureSecurityServices(this IServiceCollection services)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options => {
                options.Authority = GetVariable("AUTHORITY");
                options.Audience = GetVariable("JWT_AUDIENCE");

                options.TokenValidationParameters.ValidAlgorithms = [SecurityAlgorithms.RsaSha256];
                options.TokenValidationParameters.ValidTypes = GetVariable("JWT_TYPES")?.Split(';');

                // options.TokenValidationParameters = new TokenValidationParameters
                // {
                //     NameClaimType = "given_name",
                //     RoleClaimType ="role",

                //     // prevents confusion attacks where arbitrary tokens were accepted by api
                //     ValidTypes = new [] {"at+jwt"}
                // };
            });

        services.AddAuthorizationBuilder()
            .AddPolicy("Read", policy => { 
                policy.RequireClaim("scope", "resource.api.read", "resource.api.all");
            })
            .AddPolicy("Write", policy => { 
                policy.RequireClaim("scope", "resource.api.write", "resource.api.all");
            })
            .AddPolicy("Update", policy => { 
                policy.RequireClaim("scope", "resource.api.update", "resource.api.all");
            })
            .AddPolicy("Delete", policy => { 
                policy.RequireClaim("scope", "resource.api.delete", "resource.api.all");
            });

        // services.AddAuthorization(options =>
        // {
        //     options.AddPolicy("Read", policy => { 
        //         policy.RequireClaim("scope", "resource.api.read", "resource.api.all");
        //     });

        //     options.AddPolicy("Write", policy => { 
        //         policy.RequireClaim("scope", "resource.api.write", "resource.api.all");
        //     });

        //     options.AddPolicy("Update", policy => { 
        //         policy.RequireClaim("scope", "resource.api.update", "resource.api.all");
        //     });

        //     options.AddPolicy("Delete", policy => { 
        //         policy.RequireClaim("scope", "resource.api.delete", "resource.api.all");
        //     });
        //     // options.AddPolicy("CanSearch", policyBuilder =>
        //     // {
        //     //     policyBuilder.RequireAssertion(authHandlerContext =>
        //     //     {
        //     //         var singleOrDefault = authHandlerContext.User.Claims.SingleOrDefault(r => r.Type == "subscriberSince");
        //     //         if (authHandlerContext.User.Identity != null && singleOrDefault != null && DateTimeOffset.TryParse(singleOrDefault.Value.Trim('"'), out var value) && authHandlerContext.User.Identity.IsAuthenticated)
        //     //         {
        //     //             return DateTimeOffset.Compare(value, new DateTime(2023, 1, 1)) < 0;
        //     //         }
        //     //         else
        //     //         {
        //     //             return false;
        //     //         }
        //     //     });
        //     // });
        //     // options.AddPolicy("ClientAppCanWrite", policyBuilder =>
        //     // {
        //     //     policyBuilder.RequireClaim("scope", "notesapi.write");
        //     // });
        //     // options.AddPolicy("MustOwnResource", policyBuilder =>
        //     // {
        //     //     policyBuilder.RequireAuthenticatedUser();
        //     //     policyBuilder.AddRequirements(new MustOwnResourceRequirement());
        //     // });
        // });

        services.AddCors(options => options.AddPolicy("BFF.Proxy", builder =>
        {
            builder.AllowAnyOrigin() //TODO
                .AllowAnyMethod()
                .AllowAnyHeader();
        }));
    }

    internal static void UseSwaggerConfiguration(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(options => 
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "Resource Api");
            options.OAuthClientId(GetVariable("CLIENT_ID"));
            options.OAuthClientSecret(GetVariable("CLIENT_SECRET"));
            options.OAuthAppName(GetVariable("JWT_AUDIENCE"));
            options.OAuthScopeSeparator(",");
            options.OAuthUsePkce();
        });
    }

    private static string GetVariable(string key)
    {
        return Environment.GetEnvironmentVariable(key)?? string.Empty;
    }
}