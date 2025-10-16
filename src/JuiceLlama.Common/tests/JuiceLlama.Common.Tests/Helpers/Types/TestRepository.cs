using JuiceLlama.Common.Tests.Helpers.Interfaces;

namespace JuiceLlama.Common.Tests.Helpers.Types
{
    internal class TestRepository : ITestRepository
    {
        public async Task<string> CallAsync(string value)
        {
            return "Decorated";
        }
    }
}