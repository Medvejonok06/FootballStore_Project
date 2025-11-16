using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FootballStore.Api.Middleware
{
    public class ValidationExceptionMiddleware : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            if (context.Exception is ValidationException validation)
            {
                var problemDetails = new ValidationProblemDetails()
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Validation failed",
                    Detail = validation.Message
                };

                foreach (var error in validation.Errors)
                {
                    problemDetails.Errors.Add(error.PropertyName, new[] { error.ErrorMessage });
                }

                context.Result = new BadRequestObjectResult(problemDetails);
                context.ExceptionHandled = true;
            }
        }
    }
}
