using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using GestorDeUsuarios.Infrastructure.Data;
using System.Data.Common;
using Microsoft.Data.Sqlite;

namespace GestorDeUsuarios.Tests.IntegrationTests;

public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remover el DbContext de producción
            var dbContextDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));

            if (dbContextDescriptor != null)
                services.Remove(dbContextDescriptor);

            // Remover la conexión de base de datos de producción
            var dbConnectionDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbConnection));

            if (dbConnectionDescriptor != null)
                services.Remove(dbConnectionDescriptor);

            // Configurar base de datos en memoria para tests
            var connectionString = "DataSource=:memory:";
            services.AddSingleton<DbConnection>(container =>
            {
                var connection = new SqliteConnection(connectionString);
                connection.Open();
                return connection;
            });

            services.AddDbContext<AppDbContext>((container, options) =>
            {
                var connection = container.GetRequiredService<DbConnection>();
                options.UseSqlite(connection);
            });
        });

        builder.UseEnvironment("Testing");
    }
}