using Microsoft.EntityFrameworkCore;
using Resource.Api;
using Resource.Api.DbContexts;
using Resource.Api.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

builder.Services.AddDbContext<ApiContext>(options => {
    options.UseInMemoryDatabase(databaseName: "ResourceDb");
});

builder.Services.AddScoped<IProgrammingLanguagesRepository, 
    ProgrammingLanguagesRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

Seed.Initialize(app.Services);

app.Run();