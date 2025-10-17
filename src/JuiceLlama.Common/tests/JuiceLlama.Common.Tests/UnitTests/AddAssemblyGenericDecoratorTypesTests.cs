using JuiceLlama.Common.Extensions;
using JuiceLlama.Common.Interfaces;
using JuiceLlama.Common.Tests.Helpers.Types;
using JuiceLlama.Common.Tests.Helpers.Types.Dto;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace JuiceLlama.Common.Tests.UnitTests
{
    public class AddAssemblyGenericDecoratorTypesTests
    {
        private ServiceCollection _services;

        public AddAssemblyGenericDecoratorTypesTests()
        {
            _services = new ServiceCollection();
        }

        [Fact]
        public async Task AddAssemblyDecoratorTypes_RegisterInDI_RegistredTypesInDI()
        {
            // Arrage & Act
            _services.AddAssemblyTypes(Assembly.GetExecutingAssembly(), typeof(IRepository<,>));
            _services.AddAssemblyDecoratorTypes<ISpicificDecorator>(Assembly.GetExecutingAssembly(), typeof(IRepository<,>));
            using var scope = _services.BuildServiceProvider();

            // Assert
            var serviceB = scope.GetRequiredService<IRepository<BRequest, BResponse>>();
            var serviceC = scope.GetRequiredService<IRepository<CRequest, CResponse>>();

            Assert.NotNull(serviceB);
            Assert.NotNull(serviceC);

            var resultDecorator = await serviceB.ExecuteAsync(new BRequest());
            var resultDecorated = await serviceC.ExecuteAsync(new CRequest());

            Assert.Equal("Decorated", resultDecorated.Errors[0].Message);
            Assert.Equal("Decorator", resultDecorator.Errors[0].Message);
        }

        [Fact]
        public async Task AddAssemblyDecoratorTypes_GenericDecorator_RegistredTypesInDI()
        {
            // Arrage & Act
            _services.AddAssemblyTypes(Assembly.GetExecutingAssembly(), typeof(IRepository<,>));
            _services.AddAssemblyDecoratorTypes<IGenericDecorator>(Assembly.GetExecutingAssembly(), typeof(IRepository<,>));
            using var scope = _services.BuildServiceProvider();

            // Assert
            var serviceA = scope.GetRequiredService<IRepository<ARequest, AResponse>>();
            var serviceB = scope.GetRequiredService<IRepository<BRequest, BResponse>>();
            var serviceC = scope.GetRequiredService<IRepository<CRequest, CResponse>>();

            Assert.NotNull(serviceA);
            Assert.NotNull(serviceB);
            Assert.NotNull(serviceC);

            var resultA = await serviceA.ExecuteAsync(new ARequest());
            var resultB = await serviceB.ExecuteAsync(new BRequest());
            var resultC = await serviceC.ExecuteAsync(new CRequest());

            Assert.Equal(nameof(GenericRepositoryDecorator<ARequest, AResponse>), resultA.Errors[0].Message);
            Assert.Equal(nameof(GenericRepositoryDecorator<BRequest, BResponse>), resultA.Errors[0].Message);
            Assert.Equal(nameof(GenericRepositoryDecorator<CRequest, CResponse>), resultA.Errors[0].Message);
        }
    }
}