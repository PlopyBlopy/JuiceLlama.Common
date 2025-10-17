using FluentResults;
using JuiceLlama.Common.Interfaces;
using JuiceLlama.Common.Tests.Helpers.Types.Dto;

namespace JuiceLlama.Common.Tests.Helpers.Types
{
    internal class CRepositoryDecorator : IRepository<CRequest, CResponse>, ISpicificDecorator
    {
        private readonly IRepository<CRequest, CResponse> _decorated;

        public CRepositoryDecorator(IRepository<CRequest, CResponse> decorated)
        {
            _decorated = decorated;
        }

        public async Task<Result<CResponse>> ExecuteAsync(CRequest request, CancellationToken ct = default)
        {
            // Before code

            var result = await _decorated.ExecuteAsync(request, ct);

            // After code

            return result;
        }
    }
}