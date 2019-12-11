using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.Extensions.Logging;

namespace Archetypical.Software.Spigot.Extensions
{
    /// <summary>
    /// Extension methods to build your spigot
    /// </summary>
    public static class SpigotExtensions
    {
        /// <summary>
        /// Adds Spigot to your application and prepares it for customization
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static ISpigotBuilder AddSpigot(this IServiceCollection services, IConfiguration configuration)
        {
            var builder = new SpigotBuilder()
            {
                Configuration = configuration,
                Services = services,
            };
            services.AddSingleton<Spigot>();
            services.AddSingleton(typeof(MessageSender<>));
            return builder;
        }

        /// <summary>
        /// Customize your resilience settings for <see cref="Spigot"/>
        /// </summary>
        /// <param name="src">An instance of <see cref="ISpigotBuilder"/></param>
        /// <param name="resilienceBuilder">An action to build resilience</param>
        /// <returns></returns>
        public static ISpigotBuilder WithResilience(this ISpigotBuilder src, Action<Resilience> resilienceBuilder)
        {
            var resilience = new Resilience();
            resilienceBuilder(resilience);
            src.Resilience = resilience;
            return src;
        }

        public static ISpigotBuilder AddBeforeSend(this ISpigotBuilder src, Action<Envelope> beforeSendAction)
        {
            src.BeforeSend = beforeSendAction;
            return src;
        }

        public static ISpigotBuilder AddAfterReceive(this ISpigotBuilder src, Action<Envelope> afterReceiveAction)
        {
            src.AfterReceive = afterReceiveAction;
            return src;
        }

        public static ISpigotBuilder WithFriendlyName(this ISpigotBuilder src, string friendlyName)
        {
            src.ApplicationName = friendlyName;
            return src;
        }

        public static ISpigotBuilder AddKnob<TKnob, T>(this ISpigotBuilder src) where TKnob : Knob<T> where T : class, new()
        {
            var name = typeof(TKnob).FullName;
            src.Services.AddSingleton<TKnob>();
            src.Knobs.Add(typeof(TKnob));
            return src;
        }

        public static ISpigotBuilder AddKnob<TKnob, T>(this ISpigotBuilder src, TKnob instance) where TKnob : Knob<T> where T : class, new()
        {
            var name = typeof(TKnob).FullName;
            src.Services.AddSingleton(instance);
            src.Knobs.Add(typeof(TKnob));
            return src;
        }
    }
}