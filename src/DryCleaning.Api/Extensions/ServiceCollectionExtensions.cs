using DryCleaning.Application.Services;
using DryCleaning.Contracts.Interfaces;
using DryCleaning.Domain.Interfaces;
using DryCleaning.Domain.Seeds;
using DryCleaning.Infrastructure.InMemory;

namespace DryCleaning.Api.Extensions;

/// <summary>
/// Регистрация сервисов приложения и репозиториев в контейнере зависимостей
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Регистрирует общее хранилище в памяти с данными генератора
    /// </summary>
    public static IServiceCollection AddInMemoryRepositories(this IServiceCollection services)
    {
        var data = DryCleaningDataSeeder.Create();
        services.AddSingleton<ICategoryRepository>(_ => new InMemoryCategoryRepository(data.Categories));
        services.AddSingleton<IClientRepository>(_ => new InMemoryClientRepository(data.Clients));
        services.AddSingleton<IItemRepository>(_ => new InMemoryItemRepository(data.Items));
        services.AddSingleton<IOrderRepository>(_ => new InMemoryOrderRepository(data.Orders));
        return services;
    }

    /// <summary>
    /// Регистрирует отдельные сервисы для категорий, клиентов, изделий и заказов
    /// </summary>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IClientService, ClientService>();
        services.AddScoped<IItemService, ItemService>();
        services.AddScoped<IOrderService, OrderService>();
        return services;
    }
}
