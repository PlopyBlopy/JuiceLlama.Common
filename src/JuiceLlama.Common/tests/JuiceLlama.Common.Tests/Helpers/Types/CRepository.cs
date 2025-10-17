using FluentResults;
using JuiceLlama.Common.Interfaces;
using JuiceLlama.Common.Tests.Helpers.Types.Dto;

namespace JuiceLlama.Common.Tests.Helpers.Types
{
    internal class CRepository : IRepository<CRequest, CResponse>
    {
        public async Task<Result<CResponse>> ExecuteAsync(CRequest request, CancellationToken ct = default)
        {
            return Result.Fail<CResponse>("Decorated");
        }
    }
}