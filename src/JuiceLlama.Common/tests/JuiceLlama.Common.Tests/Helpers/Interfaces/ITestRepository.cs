namespace JuiceLlama.Common.Tests.Helpers.Interfaces
{
    internal interface ITestRepository : IRepository
    {
        Task<string> CallAsync(string value);
    }
}