using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Archetypical.Software.Spigot.Extensions
{
    public static class SpigotExtensions
    {
        public static ISpigotBuilder AddSpigot(this IServiceCollection services, IConfiguration configuration, Action<SpigotSettings> options)
        {
            var builder = new SpigotBuilder()
            {
                Configuration = configuration,
                Services = services,
                Settings = new SpigotSettings()
            };
            options?.Invoke(builder.Settings);
            return builder;
        }
    }
}