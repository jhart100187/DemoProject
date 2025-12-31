
using Microsoft.Extensions.DependencyInjection;
using DemoProject.Domain.Repositories.Users;
using DemoProject.Domain.Repositories.Products;
using DemoProject.Domain.Repositories.ShoppingCarts;

namespace DemoProject.Domain;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDomainServices(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserAddressRepository, UserAddressRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IProductPriceRepository, ProductPriceRepository>();
        services.AddScoped<IShoppingCartRepository, ShoppingCartRepository>();

        services.AddSingleton<INpgsqlConnectionFactory, NpgsqlConnectionFactory>();

        return services;
    }
}