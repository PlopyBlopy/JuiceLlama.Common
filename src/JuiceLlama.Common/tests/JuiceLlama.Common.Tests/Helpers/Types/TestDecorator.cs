using JuiceLlama.Common.Tests.Helpers.Interfaces;

namespace JuiceLlama.Common.Tests.Helpers.Types
{
    internal class TestDecorator : ITestRepository, ITestDecorator
    {
        private readonly ITestRepository _decorated;

        public TestDecorator(ITestRepository decorated)
        {
            _decorated = decorated;
        }

        public async Task<string> CallAsync(string value)
        {
            if (value.Equals("Decorated", StringComparison.OrdinalIgnoreCase))
                return await _decorated.CallAsync(value);
            else
                return "Decorator";
        }
    }
}