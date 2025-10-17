using FluentResults;

namespace JuiceLlama.Common.Interfaces
{
    // Все Repository - теперь наследуются от IRepository<TRequest, TResponse> - указывая соответствующие типы
    // Как в MediatoR - сканирование сборки, добавление всех типов что наследуются от IRepository<,>
    // Позволяет определить как общий Decorator, так и оставить конкретный относительно указанных TRequest и TResponse

    //Критерии необходимого минимума:
    /*
        1. Проверить регистрацию и вызов репозиторием наследуемых от IRepository<,>
            ATestRepository, BTestRepository, CTestRepository

        2. Проверить регистрацию и вызов универсального декоратора для разных репозиторием
            GenericDecorator -> ATestRepository, GenericDecorator -> BTestRepository

        3. Проверить регистрацию и вызов конкретного декоратора для конкретного репозитория
            ATestRepositoryDecorator -> ATestRepository, BTestRepositoryDecorator -> BTestRepository

        4. Проверить цепочку вызова декораторов сначала универсальный
            GenericDecorator -> ATestRepositoryDecorator -> ATestRepository

        5. Проверить цепочку вызова декораторов сначала конкретный
            ATestRepositoryDecorator -> GenericDecorator -> ATestRepository
    */

    public interface IRepository<TRequest, TResponse>
    {
        Task<Result<TResponse>> ExecuteAsync(TRequest request, CancellationToken ct = default);
    }
}