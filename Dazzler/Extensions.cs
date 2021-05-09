#if (NET472_OR_GREATER || NET5_0_OR_GREATER || NETCOREAPP)

using System;
using Microsoft.Extensions.DependencyInjection;

namespace Dazzler
{
   public static class DazzlerExtensions
   {
      public static IServiceCollection AddDazzler<T>(this IServiceCollection services, Action<DbContextOptions> options) where T : DbContext
      {
         if (services == null) throw new ArgumentNullException(nameof(services));

         // create an options instance to pass it to user code.
         DbContextOptions args = new DbContextOptions(services);

         // invoke the user code to manipulate options.
         options?.Invoke(args);

         // create a DbContext and add to the service collection.
         services.AddScoped<T>(x => (T)Activator.CreateInstance(typeof(T), args));
         
         return services;
      }
   }
}

#endif
