using System.Security.Claims;
using IdentityModel;
using IDP.WebApp.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace IDP.WebApp;

public class SeedData
{
    public static void EnsureSeedData(WebApplication app)
    {
        using var scope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        context.Database.Migrate();

        var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

        var testUser = userMgr.FindByNameAsync("testUser").Result;

        if (testUser == null)
        {
            testUser = new IdentityUser
            {
                UserName = "testUser",
                Email = "testUser@email.com",
                EmailConfirmed = true,
            };

            var result = userMgr.CreateAsync(testUser, "testPassw0rd$").Result;
            if (!result.Succeeded)
            {
                throw new Exception(result.Errors.First().Description);
            }

            result = userMgr.AddClaimsAsync(testUser, 
            [
                new Claim(JwtClaimTypes.Name, "TestUser"),
                new Claim(JwtClaimTypes.GivenName, "Test"),
                new Claim(JwtClaimTypes.FamilyName, "User"),
                new Claim(JwtClaimTypes.WebSite, "https://github.com/Jomaroflo94")
            ]).Result;

            if (!result.Succeeded)
            {
                throw new Exception(result.Errors.First().Description);
            }

            Log.Debug("Test User has been created!");
        }
        else
        {
            Log.Debug("Test User already exists!");
        }
    }
}
