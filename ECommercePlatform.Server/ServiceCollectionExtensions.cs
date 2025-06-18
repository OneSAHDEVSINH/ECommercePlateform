using ECommercePlatform.Application.Interfaces;
using ECommercePlatform.Infrastructure.Repositories;

namespace ECommercePlatform.Server
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterRepositories(this IServiceCollection services)
        {
            // Use reflection to find all repositories and register them
            var assembly = typeof(CountryRepository).Assembly;
            var repositoryTypes = assembly.GetTypes()
                .Where(t => t.Name.EndsWith("Repository") && !t.IsInterface && !t.IsAbstract)
                .Where(t => t != typeof(GenericRepository<>));

            foreach (var type in repositoryTypes)
            {
                var interfaces = type.GetInterfaces()
                    .Where(i => i.Name.EndsWith("Repository") && i != typeof(IGenericRepository<>))
                    .ToList();

                foreach (var iface in interfaces)
                {
                    services.AddScoped(iface, type);
                }
            }

            return services;
        }
    }
}
