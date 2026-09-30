using Exo.WebApi.Contexts;
using Exo.WebApi.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// The "Testing" environment is used by the integration tests, which register
// an in-memory database themselves. Everywhere else we use SQL Server.
if (!builder.Environment.IsEnvironment("Testing"))
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException(
            "Connection string 'DefaultConnection' was not found. " +
            "Set it in appsettings.json or in the ConnectionStrings__DefaultConnection environment variable.");

    builder.Services.AddDbContext<ExoContext>(options => options.UseSqlServer(connectionString));
}

builder.Services.AddScoped<IProjetoRepository, ProjetoRepository>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Shows the /// <summary> comments of the controllers in the Swagger UI.
    var xmlPath = Path.Combine(AppContext.BaseDirectory, $"{typeof(Program).Assembly.GetName().Name}.xml");
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});
builder.Services.AddHealthChecks();

var app = builder.Build();

// Creates the schema on startup when enabled (used by docker-compose).
// For a production system, EF Core migrations would replace this.
if (app.Configuration.GetValue<bool>("Database:EnsureCreated"))
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ExoContext>();
    context.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();

// Makes the Program class visible to WebApplicationFactory in the test project.
public partial class Program
{
}
