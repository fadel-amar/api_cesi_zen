using System.Net;
using System.Text.Json;
using CesiZen_API.Helper.Exceptions;
using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;

namespace CesiZen_API.Middleware
{

    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
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
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var response = context.Response;
            response.ContentType = "application/json";

            var baseMySqlException = GetInnermostException<MySqlException>(exception);

            HttpStatusCode statusCode;
            string message;

            if (baseMySqlException != null)
            {
                if (baseMySqlException.IsTransient)
                {
                    statusCode = HttpStatusCode.ServiceUnavailable;
                    message = "Erreur temporaire lors de la connexion à la base de données. Veuillez réessayer.";
                }
                else
                {
                    statusCode = HttpStatusCode.BadRequest;
                    message = baseMySqlException.Message;
                }
            }
            else
            {
                switch (exception)
                {
                    case NotFoundException:
                        statusCode = HttpStatusCode.NotFound;
                        message = exception.Message;
                        break;

                    case BadRequestException:
                        statusCode = HttpStatusCode.BadRequest;
                        message = exception.Message;
                        break;

                    case UnauthorizedAccessException:
                        statusCode = HttpStatusCode.Unauthorized;
                        message = exception.Message;
                        break;

                    case DbUpdateException:
                        statusCode = HttpStatusCode.BadRequest;
                        message = "Erreur lors de l'enregistrement dans la base de données. Vérifiez vos données.";
                        break;
                    default:
                        statusCode = HttpStatusCode.InternalServerError;
                        message = "Une erreur inattendue est survenue.";
                        break;
                }
            }

            var environnement = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            object errorResponse = environnement == "dev"
                ? new
                {
                    status = (int)statusCode,
                    message,
                    details = new
                    {
                        exception = exception.GetType().Name,
                        error = exception.Message,
                        stackTrace = exception.StackTrace,
                        innerException = exception.InnerException?.Message
                    }
                }
                : new
                {
                    status = (int)statusCode,
                    message
                };
            var result = JsonSerializer.Serialize(errorResponse);


            response.StatusCode = (int)statusCode;
            _logger.LogError(exception, "Exception interceptée");

            return response.WriteAsync(result);
        }

        private static TException? GetInnermostException<TException>(Exception ex) where TException : Exception
        {
            while (ex != null)
            {
                if (ex is TException matched)
                    return matched;

                ex = ex.InnerException!;
            }

            return null;
        }



    }
}
