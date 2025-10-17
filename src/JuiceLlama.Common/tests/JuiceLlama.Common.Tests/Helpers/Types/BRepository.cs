using FluentResults;
using JuiceLlama.Common.Interfaces;
using JuiceLlama.Common.Tests.Helpers.Types.Dto;

namespace JuiceLlama.Common.Tests.Helpers.Types
{
    internal class BRepository : IRepository<BRequest, BResponse>
    {
        public async Task<Result<BResponse>> ExecuteAsync(BRequest request, CancellationToken ct = default)
        {
            return Result.Ok(new BResponse());
        }
    }
}