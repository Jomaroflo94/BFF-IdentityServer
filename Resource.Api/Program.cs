using Resource.Api;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

Log.Information("Starting up");

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.ConfigureHost();

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddControllers();

    builder.Services.ConfigureSwaggerServices();
    builder.Services.ConfigureDbContextServices();
    builder.Services.ConfigureRepositoriesServices();
    builder.Services.ConfigureSecurityServices();

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwaggerConfiguration();
    }

    app.UseCors("BFF.Proxy");
    app.UseHttpsRedirection();
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();

    Seed.Initialize(app.Services);

    app.Run();
}
catch (Exception ex) when (ex is not HostAbortedException)
{
    Log.Fatal(ex, "Unhandled exception");
}
finally
{
    Log.Information("Shut down complete");
    Log.CloseAndFlush();
}