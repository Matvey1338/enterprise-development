using System.ComponentModel.DataAnnotations;
using DryCleaning.Application.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace DryCleaning.Api.Infrastructure;

/// <summary>
/// Преобразует ошибки в ответы API, скрывая внутренние детали непредвиденных сбоев
/// </summary>
/// <param name="logger">Журнал ошибок</param>
public class ApiExceptionHandler(ILogger<ApiExceptionHandler> logger) : IExceptionHandler
{
    /// <inheritdoc/>
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var statusCode = exception switch
        {
            ValidationException => StatusCodes.Status400BadRequest,
            EntityInUseException => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status500InternalServerError,
        };

        if (statusCode == StatusCodes.Status500InternalServerError)
        {
            logger.LogError(exception, "Ошибка при обработке {Method} {Path}",
                httpContext.Request.Method, httpContext.Request.Path);
        }
        else
        {
            logger.LogWarning(exception, "Запрос {Method} {Path} отклонён: {StatusCode}",
                httpContext.Request.Method, httpContext.Request.Path, statusCode);
        }

        ProblemDetails problem = exception is ValidationException validation
            ? new ValidationProblemDetails(new Dictionary<string, string[]>
            {
                [validation.ValidationResult?.MemberNames.FirstOrDefault() ?? string.Empty] = [exception.Message],
            })
            : new ProblemDetails();
        problem.Status = statusCode;
        problem.Title = statusCode switch
        {
            StatusCodes.Status400BadRequest => "Некорректные данные запроса.",
            StatusCodes.Status409Conflict => "Удаление невозможно.",
            _ => "Произошла внутренняя ошибка сервера.",
        };
        problem.Detail = statusCode == StatusCodes.Status500InternalServerError ? null : exception.Message;
        problem.Extensions["traceId"] = httpContext.TraceIdentifier;
        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(problem, problem.GetType(), options: null,
            contentType: "application/problem+json", cancellationToken: cancellationToken);
        return true;
    }
}
