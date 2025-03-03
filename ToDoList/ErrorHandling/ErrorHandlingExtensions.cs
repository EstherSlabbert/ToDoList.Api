using Microsoft.AspNetCore.Diagnostics;
using ToDoList.Repositories;
using ToDoList.Entities;
using ToDoList.Common;

namespace ToDoList.ErrorHandling;

public static class ErrorHandlingExtensions
{
    public static IApplicationBuilder MapExceptionsToProblemDetails(this IApplicationBuilder app)
    {
        app.UseExceptionHandler(exceptionHandlerApp =>
        {
            exceptionHandlerApp.Run(async context =>
            {
                var exception = context.Features.Get<IExceptionHandlerFeature>()!.Error;
                var problem = await BuildProblemDetails(exception, context);
                await problem.ExecuteAsync(context);
            });
        });
        return app;
    }

    private static async ValueTask<IResult> BuildProblemDetails(Exception exception, HttpContext httpContext)
    {
        return exception switch
        {
            AccessForbiddenException accessForbiddenException => Results.Problem(
                statusCode: StatusCodes.Status403Forbidden,
                detail: accessForbiddenException.Message,
                title: "Access Forbidden"),
            EntityNotFoundException aggregateNotFoundException => Results.Problem(
                statusCode: StatusCodes.Status404NotFound,
                detail: aggregateNotFoundException.Message),
            BusinessException businessException => Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                detail: businessException.Message,
                title: "Business Exception"),
            _ => await RecordUnhandledExceptionAndProblemDetails(exception, httpContext)
        };
    }

    private static async Task<IResult> RecordUnhandledExceptionAndProblemDetails(Exception exception, HttpContext httpContext)
    {
        var unhandledExceptionsRecorder = httpContext.RequestServices.GetRequiredService<IUnhandledExceptionRepository>();

#pragma warning disable CS8601 // Possible null reference assignment.
        var unhandledException = new UnhandledException
        {
            Message = exception.Message,
            StackTrace = exception.StackTrace,
            Source = exception.Source,
            RequestPath = httpContext.Request.Path,
            RequestMethod = httpContext.Request.Method
        };
#pragma warning restore CS8601 // Possible null reference assignment.

        var exceptionId = await unhandledExceptionsRecorder.RecordAsync(unhandledException);

#pragma warning disable CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.
        return Results.Problem(statusCode: StatusCodes.Status500InternalServerError,
                               extensions: new Dictionary<string, object> { { "exceptionId", exceptionId } });
#pragma warning restore CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.
    }
}
