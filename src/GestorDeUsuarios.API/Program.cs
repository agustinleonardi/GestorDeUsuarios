using GestorDeUsuarios.Domain.Abstracciones.Repositorios;
using GestorDeUsuarios.Infrastructure.Data;
using GestorDeUsuarios.Infrastructure.Mappings;
using GestorDeUsuarios.Infrastructure.Repositorios;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

DotNetEnv.Env.Load("../../.env");
var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");

if (string.IsNullOrWhiteSpace(connectionString))
    throw new InvalidOperationException("No se encontró la variable DB_CONNECTION_STRING.");

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IDomicilioRepository, DomicilioRepository>();
builder.Services.AddAutoMapper(typeof(DomainToEntityProfile).Assembly);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.Migrate();

    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Run();

