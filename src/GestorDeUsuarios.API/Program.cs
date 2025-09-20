using FluentValidation;
using FluentValidation.AspNetCore;
using GestorDeUsers.Infrastructure.Repositories;
using GestorDeUsuarios.Application.Abstractions.UsesCases;
using GestorDeUsuarios.Application.Mappings;
using GestorDeUsuarios.Application.UsesCases;
using GestorDeUsuarios.Application.Validators;
using GestorDeUsuarios.Domain.Abstractions.Repositories;
using GestorDeUsuarios.Infrastructure.Data;
using GestorDeUsuarios.Infrastructure.Mappings;
using GestorDeUsuarios.Infrastructure.Repositories;
using GestorDeUsuarios.API.Middleware;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

DotNetEnv.Env.Load("../../.env");
var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");

if (string.IsNullOrWhiteSpace(connectionString))
    throw new InvalidOperationException("No se encontró la variable DB_CONNECTION_STRING.");

// Configurar Controllers
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// Repositorios
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAddressRepository, AddressRepository>();

// Use Cases
builder.Services.AddScoped<ICreateUserUseCase, CreateUserUseCase>();
builder.Services.AddScoped<IGetUserByIdUseCase, GetUserByIdUseCase>();
builder.Services.AddScoped<ISearchUsersUseCase, SearchUsersUseCase>();
builder.Services.AddScoped<IUpdateUserUseCase, UpdateUserUseCase>();
builder.Services.AddScoped<IDeleteUserUseCase, DeleteUserUseCase>();

builder.Services.AddAutoMapper(
    typeof(DomainToEntityProfile).Assembly,     // Infrastructure mappings
    typeof(DomainToResponseProfile).Assembly    // Application mappings
);

builder.Services.AddValidatorsFromAssemblyContaining<CreateUserRequestValidator>();
builder.Services.AddFluentValidationAutoValidation();

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

// Middleware de manejo global de excepciones
app.UseMiddleware<GlobalExceptionMiddleware>();

// Mapear controllers
app.MapControllers();

app.Run();

// Hacer la clase Program accesible para tests de integración
public partial class Program { }

