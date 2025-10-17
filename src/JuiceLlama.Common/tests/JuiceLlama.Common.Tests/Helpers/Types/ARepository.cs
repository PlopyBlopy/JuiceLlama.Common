using FluentResults;
using JuiceLlama.Common.Interfaces;
using JuiceLlama.Common.Tests.Helpers.Types.Dto;

namespace JuiceLlama.Common.Tests.Helpers.Types
{
    internal class ARepository : IRepository<ARequest, AResponse>
    {
        public async Task<Result<AResponse>> ExecuteAsync(ARequest request, CancellationToken ct = default)
        {
            return Result.Ok(new AResponse());
        }
    }
}