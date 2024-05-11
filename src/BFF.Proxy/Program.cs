using Duende.Bff;
using Duende.Bff.Yarp;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers();

// Register BFF services and configure the BFF middleware
// Registra servicios necesarios para el reenvio http de YARP
builder.Services.AddBff().AddRemoteApis();

builder.Services
    // Configure ASP.NET Authentication
    .AddAuthentication(options => 
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
        options.Cookie.Name = "__MySPA";
        // When the value is Strict the cookie will only be sent along with "same-site" requests.
        options.Cookie.SameSite = SameSiteMode.Strict;
    })
    // Configure OpenID Connect
    .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options => 
    {
        options.Authority = Environment.GetEnvironmentVariable("AUTH_AUTHORITY");
        options.ClientId = Environment.GetEnvironmentVariable("AUTH_CLIENT_ID");
        options.ClientSecret = Environment.GetEnvironmentVariable("AUTH_CLIENT_SECRET");

        options.ResponseType = OpenIdConnectResponseType.Code;
        options.ResponseMode = OpenIdConnectResponseMode.Query;
        //options.UsePkce = true;

        // Go to the user info endpoint to retrieve additional claims after creating an identity from the id_token
        options.GetClaimsFromUserInfoEndpoint = true;
        // Store access and refresh tokens in the authentication cookie
        options.SaveTokens = true;

        options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();

        options.Scope.Clear();
        options.Scope.Add(OpenIdConnectScope.OpenId);
        options.Scope.Add(OpenIdConnectScope.OpenIdProfile);
        options.Scope.Add(OpenIdConnectScope.OfflineAccess);
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
// Use the BFF middleware (must be before UseAuthorization)
app.UseBff();
app.UseAuthorization();

app.MapControllers().AsBffApiEndpoint();

// Adds the BFF management endpoints (/bff/login, /bff/logout, ...)
app.MapBffManagementEndpoints();

app.MapRemoteBffApiEndpoint(
        "/api/notes", "https://localhost:7094/api/Note/GetNotes")
    .RequireAccessToken(TokenType.User);

// var summaries = new[]
// {
//     "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
// };

// app.MapGet("/weatherforecast", () =>
// {
//     var forecast =  Enumerable.Range(1, 5).Select(index =>
//         new WeatherForecast
//         (
//             DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
//             Random.Shared.Next(-20, 55),
//             summaries[Random.Shared.Next(summaries.Length)]
//         ))
//         .ToArray();
//     return forecast;
// })
// .WithName("GetWeatherForecast")
// .AsBffApiEndpoint()
// .RequireAuthorization()
// .WithOpenApi();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
