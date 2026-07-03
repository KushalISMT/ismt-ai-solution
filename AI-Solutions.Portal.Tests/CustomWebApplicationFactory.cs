using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using AI_Solutions.Portal.Web.Data;

namespace AI_Solutions.Portal.Tests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove existing DbContext registration
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // Use a separate test database
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer("Server=.;User Id=dbadmin;Password=Irush@12345;Initial Catalog=ai-solutions-db-test;TrustServerCertificate=True;");
            });
        });
    }
}
