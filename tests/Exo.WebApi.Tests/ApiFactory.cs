using Exo.WebApi.Contexts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Exo.WebApi.Tests;

/// <summary>
/// Starts the real API in memory (routing, validation, controllers, DI)
/// but swaps SQL Server for EF Core's in-memory database.
/// </summary>
public class ApiFactory : WebApplicationFactory<Program>
{
    private readonly string _databaseName = $"ExoApiTests-{Guid.NewGuid()}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Program.cs skips the SQL Server registration in this environment.
        builder.UseEnvironment("Testing");

        builder.ConfigureTestServices(services =>
        {
            services.AddDbContext<ExoContext>(options => options.UseInMemoryDatabase(_databaseName));
        });
    }
}
