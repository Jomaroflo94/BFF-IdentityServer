using Microsoft.EntityFrameworkCore;
using Resource.Api.DbContexts;

namespace Resource.Api;

public static class Seed
{
    public static void Initialize(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<DbContextOptions<ApiContext>>();
        using var context = new ApiContext(service);

        if (context.ProgrammingLanguages.Any())
        {
            return;
        }

        context.ProgrammingLanguages.AddRange(
            new() { Id = 1, Name = "C#" },
            new() { Id = 2, Name = "Python" },
            new() { Id = 3, Name = "JavaScript" },
            new() { Id = 4, Name = "Java" },
            new() { Id = 5, Name = "Visual Basic" },
            new() { Id = 6, Name = "Go" },
            new() { Id = 7, Name = "Swift" },
            new() { Id = 8, Name = "C++" }
        );

        context.SaveChanges();
    }
}
