using FluentResults;
using JuiceLlama.Common.Interfaces;

namespace JuiceLlama.Common.Tests.Helpers.Types
{
    internal class GenericRepositoryDecorator<TRequest, TResponse> : IRepository<TRequest, TResponse>, IGenericDecorator
    {
        private readonly IRepository<TRequest, TResponse> _decorated;

        public GenericRepositoryDecorator(IRepository<TRequest, TResponse> decorated)
        {
            _decorated = decorated;
        }

        public async Task<Result<TResponse>> ExecuteAsync(TRequest request, CancellationToken ct = default)
        {
            // Before code

            var result = await _decorated.ExecuteAsync(request, ct);

            // After code

            return Result.Fail<TResponse>(nameof(GenericRepositoryDecorator<TRequest, TResponse>));
        }
    }
}