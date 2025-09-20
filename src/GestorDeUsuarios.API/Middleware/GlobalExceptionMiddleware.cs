using System.Net;
using System.Text.Json;
using GestorDeUsuarios.Domain.Exceptions;
using GestorDeUsuarios.Application.Exceptions;

namespace GestorDeUsuarios.API.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Ocurrio un error inesperado: {ex.Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var response = context.Response;
        response.ContentType = "application/json";

        var errorResponse = exception switch
        {
            // Excepciones específicas de Dominio -> 400 Bad Request
            InvalidUserDataException or InvalidAddressDataException => new ErrorResponse
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Message = "Datos inválidos",
                Details = exception.Message
            },

            // Excepciones generales de Dominio -> 400 Bad Request
            DomainException => new ErrorResponse
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Message = "Error de validación de dominio",
                Details = exception.Message
            },

            // Excepciones específicas de Aplicación
            UserAlreadyExistsException => new ErrorResponse
            {
                StatusCode = (int)HttpStatusCode.Conflict,
                Message = "Recurso ya existe",
                Details = exception.Message
            },

            UserNotFoundApplicationException => new ErrorResponse
            {
                StatusCode = (int)HttpStatusCode.NotFound,
                Message = "Recurso no encontrado",
                Details = exception.Message
            },
            InvalidSearchCriteriaException => new ErrorResponse
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Message = "Se necesita al menos un dato de busqueda",
                Details = exception.Message
            },

            // Excepciones no manejadas -> 500 Internal Server Error
            _ => new ErrorResponse
            {
                StatusCode = (int)HttpStatusCode.InternalServerError,
                Message = "Error interno del servidor",
                Details = "Ha ocurrido un error inesperado"
            }
        };

        response.StatusCode = errorResponse.StatusCode;

        var jsonResponse = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await response.WriteAsync(jsonResponse);
    }
}

// Clase para la respuesta de error
public class ErrorResponse
{
    public int StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;
    public string Details { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
