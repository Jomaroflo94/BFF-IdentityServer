using Microsoft.EntityFrameworkCore;
using Resource.Api.Entities;

namespace Resource.Api.DbContexts;

internal class ApiContext(DbContextOptions<ApiContext> options) : DbContext(options)
{
    internal DbSet<ProgrammingLanguage> ProgrammingLanguages { get; set; }
}
