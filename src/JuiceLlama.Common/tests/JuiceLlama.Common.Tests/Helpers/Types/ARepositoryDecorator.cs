using FluentResults;
using JuiceLlama.Common.Interfaces;
using JuiceLlama.Common.Tests.Helpers.Types.Dto;

namespace JuiceLlama.Common.Tests.Helpers.Types
{
    internal class ARepositoryDecorator : IRepository<ARequest, AResponse>, ISpicificDecorator
    {
        private readonly IRepository<ARequest, AResponse> _decorated;

        public ARepositoryDecorator(IRepository<ARequest, AResponse> decorated)
        {
            _decorated = decorated;
        }

        public async Task<Result<AResponse>> ExecuteAsync(ARequest request, CancellationToken ct = default)
        {
            // Before code

            var result = await _decorated.ExecuteAsync(request, ct);

            // After code

            return result;
        }
    }
}