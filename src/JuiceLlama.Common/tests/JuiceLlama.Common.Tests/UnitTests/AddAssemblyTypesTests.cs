using JuiceLlama.Common.Extensions;
using JuiceLlama.Common.Tests.Helpers.Interfaces;
using JuiceLlama.Common.Tests.Helpers.Types;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace JuiceLlama.Common.Tests.UnitTests
{
    public class AddAssemblyTypesTests
    {
        private ServiceCollection _services;

        public AddAssemblyTypesTests()
        {
            _services = new ServiceCollection();
        }

        [Fact]
        public void AddAssemblyTypes_ValidData_ContainerWithTypes()
        {
            // Arrage & Act
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
            // Arrage & Act
            _services.AddAssemblyTypes<ITestWithoutInheritance>(Assembly.GetExecutingAssembly());
            using var scope = _services.BuildServiceProvider();

            // Assert
            var service = scope.GetService<ITestRepository>();
            Assert.Null(service);
        }
    }
}