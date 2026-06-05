using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace TaskManagerPro.API.Filters;

public class ValidationFilter : IAsyncActionFilter
{
    private readonly IServiceProvider _serviceProvider;

    public ValidationFilter(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        foreach (var argument in context.ActionArguments.Values)
        {
            if (argument is null)
            {
                continue;
            }

            var validatorType = typeof(IValidator<>).MakeGenericType(argument.GetType());

            if (_serviceProvider.GetService(validatorType) is not IValidator validator)
            {
                continue;
            }

            var validationContext = new ValidationContext<object>(argument);
            var result = await validator.ValidateAsync(validationContext);

            if (!result.IsValid)
            {
                var errors = result.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.ErrorMessage).ToArray()
                    );

                var problem = new ValidationProblemDetails(errors)
                {
                    Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
                    Title = "Validation Error",
                    Status = StatusCodes.Status400BadRequest,
                    Instance = context.HttpContext.Request.Path
                };
                problem.Extensions["traceId"] = context.HttpContext.TraceIdentifier;

                context.Result = new BadRequestObjectResult(problem);
                return;
            }
        }

        await next();
    }
}
