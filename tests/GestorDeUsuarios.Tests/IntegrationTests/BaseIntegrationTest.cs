using System.Net.Http.Json;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using GestorDeUsuarios.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Xunit;

namespace GestorDeUsuarios.Tests.IntegrationTests;

public abstract class BaseIntegrationTest : IClassFixture<CustomWebApplicationFactory<Program>>, IAsyncLifetime
{
    protected readonly HttpClient _client;
    protected readonly CustomWebApplicationFactory<Program> _factory;
    protected readonly AppDbContext _dbContext;

    protected BaseIntegrationTest(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();

        // Obtener el contexto de la base de datos para verificaciones
        var scope = _factory.Services.CreateScope();
        _dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    }

    public async Task InitializeAsync()
    {
        // Crear la base de datos y aplicar migraciones antes de cada test
        await _dbContext.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        // Limpiar la base de datos después de cada test
        await _dbContext.Database.EnsureDeletedAsync();
    }

    protected static StringContent CreateJsonContent(object obj)
    {
        var json = JsonConvert.SerializeObject(obj);
        return new StringContent(json, Encoding.UTF8, "application/json");
    }

    protected async Task<T?> GetResponseContent<T>(HttpResponseMessage response)
    {
        var content = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<T>(content);
    }
}