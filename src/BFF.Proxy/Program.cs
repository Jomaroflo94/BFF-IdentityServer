using Duende.Bff;
using Duende.Bff.Yarp;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

// Register BFF services and configure the BFF middleware
// Registra servicios necesarios para el reenvio http de YARP
builder.Services.AddBff(options => {
    //options.AntiForgeryHeaderName = 
}).AddRemoteApis();

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
        options.Cookie.Name = "__ClientSPA";
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
        "/api/notes", "https://localhost:7094/api/Note/GetNotes")
    .RequireAccessToken(TokenType.User);

app.Run();