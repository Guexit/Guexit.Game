using Guexit.Game.Application.Exceptions;
using Guexit.Game.Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;

namespace Guexit.Game.WebApi.ErrorHandling;

public static class ErrorHandlingExtensions
{
    public static IApplicationBuilder MapExceptionsToProblemDetails(this IApplicationBuilder app)
    {
        app.UseExceptionHandler(exceptionHandlerApp =>
        {
            exceptionHandlerApp.Run(async context =>
            {
                var exception = context.Features.Get<IExceptionHandlerFeature>()!.Error;

                await BuildProblemDetails(exception).ExecuteAsync(context);
            });
        });

        return app;
    }

    private static IResult BuildProblemDetails(Exception exception)
    {
        return exception switch
        {
            AggregateNotFoundException aggregateNotFoundException => Results.Problem(
                statusCode: StatusCodes.Status404NotFound,
                detail: aggregateNotFoundException.Message),
            DomainException domainException => Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                detail: domainException.Message,
                title: domainException.Title),
            _ => Results.Problem()
        };
    }
}
