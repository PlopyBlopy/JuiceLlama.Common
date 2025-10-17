using JuiceLlama.Common.Extensions;
using JuiceLlama.Common.Interfaces;
using JuiceLlama.Common.Tests.Helpers.Types.Dto;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace JuiceLlama.Common.Tests.UnitTests
{
    public class AddAssemblyGenericTypesTests
    {
        private ServiceCollection _services;

        public AddAssemblyGenericTypesTests()
        {
            _services = new ServiceCollection();
        }

        [Fact]
        public void AddAssemblyTypes_RegisterInDI_RegistredTypesInDI()
        {
            // Arrage & Act
            _services.AddAssemblyTypes(Assembly.GetExecutingAssembly(), typeof(IRepository<,>));
            using var scope = _services.BuildServiceProvider();

            // Assert
            var serviceA = scope.GetRequiredService<IRepository<ARequest, AResponse>>();
            var serviceB = scope.GetRequiredService<IRepository<BRequest, BResponse>>();
            var serviceC = scope.GetRequiredService<IRepository<CRequest, CResponse>>();

            Assert.NotNull(serviceA);
            Assert.NotNull(serviceB);
            Assert.NotNull(serviceC);
        }
    }
}