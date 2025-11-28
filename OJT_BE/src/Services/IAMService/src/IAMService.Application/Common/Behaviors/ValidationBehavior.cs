namespace IAMService.Application.Common.Behaviors
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TRequest">The type of the request.</typeparam>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    /// <seealso cref="MediatR.IPipelineBehavior&lt;TRequest, TResponse&gt;" />
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        /// <summary>
        /// The validators
        /// </summary>
        private readonly IEnumerable<IValidator<TRequest>> _validators;
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationBehavior{TRequest, TResponse}"/> class.
        /// </summary>
        /// <param name="validators">The validators.</param>
        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators) =>
            _validators = validators;

        /// <summary>
        /// Pipeline handler. Perform any additional behavior and await the <paramref name="next" /> delegate as necessary
        /// </summary>
        /// <param name="request">Incoming request</param>
        /// <param name="next">Awaitable delegate for the next action in the pipeline. Eventually this delegate represents the handler.</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>
        /// Awaitable task returning the <typeparamref name="TResponse" />
        /// </returns>
        /// <exception cref="FluentValidation.ValidationException"></exception>
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (!_validators.Any()) return await next();

            var context = new ValidationContext<TRequest>(request);
            var failures = _validators
                .Select(v => v.Validate(context))
                .SelectMany(r => r.Errors)
                .Where(f => f != null)
                .ToList();

            if (failures.Count != 0)
                throw new FluentValidation.ValidationException(failures);

            return await next();
        }
    }
}
