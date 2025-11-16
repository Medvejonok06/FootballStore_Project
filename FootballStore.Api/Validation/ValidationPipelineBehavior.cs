using FluentValidation;
using MediatR;

namespace FootballStore.Services.Validation
{
    public class ValidationPipelineBehavior<TRequest, TResponse> 
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationPipelineBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            if (_validators.Any())
            {
                var context = new ValidationContext<TRequest>(request);

                var validationResults = await Task.WhenAll(
                    _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

                var errors = validationResults
                    .SelectMany(result => result.Errors)
                    .Where(f => f != null)
                    .ToList();

                if (errors.Count > 0)
                    throw new ValidationException(errors);
            }

            return await next();
        }
    }
}
