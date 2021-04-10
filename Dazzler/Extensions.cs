using System;

#if (NET472_OR_GREATER || NET5_0_OR_GREATER || NETCOREAPP)
using Microsoft.Extensions.DependencyInjection;
#endif

namespace Dazzler
{
#if (NET472_OR_GREATER || NET5_0_OR_GREATER || NETCOREAPP)

    public static class DazzlerExtensions
    {
        public static IServiceCollection AddDazzler<T>(this IServiceCollection services, Action<DbContextOptions> options) where T : DbContext
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            DbContextOptions args = new DbContextOptions();
            options?.Invoke(args);

            services.AddScoped<T>(x => (T)Activator.CreateInstance(typeof(T), args));

            return services;
        }
    }

#endif
}
