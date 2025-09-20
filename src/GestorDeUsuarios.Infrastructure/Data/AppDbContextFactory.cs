using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using GestorDeUsuarios.Infrastructure.Data;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        DotNetEnv.Env.Load("../../.env");
        var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");

        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));

        return new AppDbContext(optionsBuilder.Options);

    }
}