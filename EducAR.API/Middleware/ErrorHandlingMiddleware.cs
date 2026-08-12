using System.Net;
using System.Text.Json;

namespace EducAR.API.Middleware;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _next   = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error no controlado: {Message}", ex.Message);
            await ManejarExcepcion(context, ex);
        }
    }

    private static async Task ManejarExcepcion(HttpContext context, Exception ex)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode  = (int)HttpStatusCode.InternalServerError;

        var respuesta = new
        {
            mensaje = "Ocurrió un error interno en el servidor.",
            detalle = ex.Message
        };

        var json = JsonSerializer.Serialize(respuesta);
        await context.Response.WriteAsync(json);
    }
}