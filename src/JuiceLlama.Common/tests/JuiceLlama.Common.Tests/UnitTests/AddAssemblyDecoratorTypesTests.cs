using JuiceLlama.Common.Extensions;
using JuiceLlama.Common.Interfaces;
using JuiceLlama.Common.Tests.Helpers.Interfaces;
using JuiceLlama.Common.Tests.Helpers.Types;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace JuiceLlama.Common.Tests.UnitTests
{
    // need tests for
    // [ AddAssemblyDecoratorTypes ]

    // TODO: указать 2 интерфейса наследуемые от IDecorator -> берется 1 наследник от декоратор, но метод Handler вызывал 2 интерфейс, поэтому этот класс декоратор не вызвался -> нужно указывать только 1 класс декоратор

    // TODO: 1. проверка цепочки вызова декораторов, относительно порядка регистрации
    //     TODO: если 2 разных декоратора
    //     TODO: если 2 одинаковых декоратора
    /*
    !!! особенности порядок вызова != порядок регистрации, первый вызывается тот кто зарегестрирован позднее !!!
    ITracingDecorator -> ILoggerDecorator -> ICacheDecorator -> IRepository

    services.AddAssemblyTypes<IRepository>(Assembly.GetExecutingAssembly());
    services.AddAssemblyDecoratorTypes<ICacheDecorator, IRepository>(Assembly.GetExecutingAssembly());
    services.AddAssemblyDecoratorTypes<ILoggerDecorator, IRepository>(Assembly.GetExecutingAssembly());
    services.AddAssemblyDecoratorTypes<ITracingDecorator, IRepository>(Assembly.GetExecutingAssembly());
    */

    // TODO:
    // TODO:
    public class AddAssemblyDecoratorTypesTests
    {
        private ServiceCollection _services;

        public AddAssemblyDecoratorTypesTests()
        {
            _services = new ServiceCollection();
        }

        [Fact]
        public async Task AddAssemblyDecoratorTypes_ValidData_ContainerWithTypes()
        {
            // Arrage & Act

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
            // Arrage & Act
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
            Action exITestDecorated;
            Action exITestDecorator;

            // Act
            exITestDecorated = () => _services.AddAssemblyDecoratorTypes<ITestDecorator, TestRepository>(Assembly.GetExecutingAssembly());
            exITestDecorator = () => _services.AddAssemblyDecoratorTypes<TestDecorator, IRepository>(Assembly.GetExecutingAssembly());

            // Assert
            var exDecorated = Assert.Throws<ArgumentException>(exITestDecorated);
            var exDecorator = Assert.Throws<ArgumentException>(exITestDecorator);

            Assert.Equal("TDecorated", exDecorated.ParamName);
            Assert.Equal("TDecorator", exDecorator.ParamName);
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