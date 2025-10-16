using JuiceLlama.Common.Extensions;
using JuiceLlama.Common.Interfaces;
using JuiceLlama.Common.Tests.Helpers.Interfaces;
using JuiceLlama.Common.Tests.Helpers.Types;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace UnitTests.Common
{
    public class AddTypesFromAssemblyTests
    {
        private ServiceCollection _services;

        public AddTypesFromAssemblyTests()
        {
            _services = new ServiceCollection();
        }

        [Fact]
        public void AddAssemblyTypes_ValidData_ContainerWithTypes()
        {
            // Arrage
            // Act
            _services.AddAssemblyTypes<IRepository>(Assembly.GetExecutingAssembly());
            using var scope = _services.BuildServiceProvider();

            // Assert
            var service = scope.GetRequiredService<ITestRepository>();
            Assert.NotNull(service);
        }

        [Fact]
        public void AddAssemblyTypes_UnvalidInterface_ArgumentException()
        {
            // Arrage
            Action exCode;

            // Act
            exCode = () => _services.AddAssemblyTypes<TestRepository>(Assembly.GetExecutingAssembly());

            // Assert
            Assert.Throws<ArgumentException>(exCode);
        }

        [Fact]
        public void AddAssemblyTypes_NothingTypes_InvalidOperationException()
        {
            // Arrage
            Action exCode;

            // Act
            _services.AddAssemblyTypes<ITestWithoutInheritance>(Assembly.GetExecutingAssembly());
            using var scope = _services.BuildServiceProvider();

            exCode = () => scope.GetRequiredService<ITestRepository>();

            // Assert
            Assert.Throws<InvalidOperationException>(exCode);
        }

        [Fact]
        public void AddAssemblyTypes_NothingTypes_Null()
        {
            // Arrage
            // Act
            _services.AddAssemblyTypes<ITestWithoutInheritance>(Assembly.GetExecutingAssembly());
            using var scope = _services.BuildServiceProvider();

            // Assert
            var service = scope.GetService<ITestRepository>();
            Assert.Null(service);
        }

        [Fact]
        public async Task AddAssemblyDecoratorTypes_ValidData_ContainerWithTypes()
        {
            // Arrage
            // Act
            _services.AddAssemblyTypes<IRepository>(Assembly.GetExecutingAssembly());
            _services.AddAssemblyDecoratorTypes<IDecorator, IRepository>(Assembly.GetExecutingAssembly());

            using var scope = _services.BuildServiceProvider();

            // Assert
            var service = scope.GetRequiredService<ITestRepository>();

            var resultDecorator = await service.CallAsync("Decorator");
            var resultDecorated = await service.CallAsync("Decorated");

            Assert.NotNull(service);
            Assert.Equal("Decorator", resultDecorator);
            Assert.Equal("Decorated", resultDecorated);
        }

        [Fact]
        public async Task AddAssemblyDecoratorTypes_NothingDecorateType_NoDecorate()
        {
            // Arrage
            // Act
            _services.AddAssemblyTypes<IRepository>(Assembly.GetExecutingAssembly());
            _services.AddAssemblyDecoratorTypes<ITestWithoutInheritance, IRepository>(Assembly.GetExecutingAssembly());

            using var scope = _services.BuildServiceProvider();

            // Assert
            var service = scope.GetRequiredService<ITestRepository>();

            var resultDecorator = await service.CallAsync("Decorator");
            var resultDecorated = await service.CallAsync("Decorated");

            Assert.NotNull(service);
            Assert.NotEqual("Decorator", resultDecorator);
            Assert.Equal("Decorated", resultDecorated);
        }

        [Fact]
        public void AddAssemblyDecoratorTypes_UnvalidInterface_ArgumentException()
        {
            // Arrage
            Action exITestRepository;
            Action exITestDecorator;

            // Act
            exITestRepository = () => _services.AddAssemblyDecoratorTypes<ITestDecorator, TestRepository>(Assembly.GetExecutingAssembly());
            exITestDecorator = () => _services.AddAssemblyDecoratorTypes<TestDecorator, IRepository>(Assembly.GetExecutingAssembly());

            // Assert
            Assert.Throws<ArgumentException>(typeof(TestRepository).ToString(), exITestRepository);
            Assert.Throws<ArgumentException>(typeof(TestDecorator).ToString(), exITestDecorator);
        }

        [Fact]
        public void AddAssemblyDecoratorTypes_NothingTypes_InvalidOperationException()
        {
            // Arrage
            Action exITestRepository;
            Action exITestDecorator;

            // Act
            _services.AddAssemblyDecoratorTypes<ITestWithoutInheritance, ITestWithoutInheritance>(Assembly.GetExecutingAssembly());

            using var scope = _services.BuildServiceProvider();

            exITestRepository = () => scope.GetRequiredService<ITestRepository>();
            exITestDecorator = () => scope.GetRequiredService<ITestRepository>();

            // Assert
            Assert.Throws<InvalidOperationException>(exITestRepository);
            Assert.Throws<InvalidOperationException>(exITestDecorator);
        }
    }
}