using FluentResults;
using JuiceLlama.Common.Interfaces;
using JuiceLlama.Common.Tests.Helpers.Types.Dto;

namespace JuiceLlama.Common.Tests.Helpers.Types
{
    internal class BRepositoryDecorator : IRepository<BRequest, BResponse>, ISpicificDecorator
    {
        private readonly IRepository<BRequest, BResponse> _decorated;

        public BRepositoryDecorator(IRepository<BRequest, BResponse> decorated)
        {
            _decorated = decorated;
        }

        public async Task<Result<BResponse>> ExecuteAsync(BRequest request, CancellationToken ct = default)
        {
            // Before code

            var result = await _decorated.ExecuteAsync(request, ct);

            // After code

            return Result.Fail<BResponse>("Decorator");
        }
    }
}