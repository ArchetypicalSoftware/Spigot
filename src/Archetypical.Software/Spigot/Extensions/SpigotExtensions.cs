using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using CloudNative.CloudEvents;

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

        /// <summary>
        /// Allows for intercepting the message before sending. This can be used for advanced scenarios
        /// </summary>
        /// <param name="src"></param>
        /// <param name="beforeSendAction"></param>
        /// <returns></returns>
        public static ISpigotBuilder AddBeforeSend(this ISpigotBuilder src, Action<CloudEvent> beforeSendAction)
        {
            src.BeforeSend = beforeSendAction;
            return src;
        }

        /// <summary>
        /// Allows for intercepting the message after receipt but before entering the pipeline
        /// </summary>
        /// <param name="src"></param>
        /// <param name="afterReceiveAction"></param>
        /// <returns></returns>
        public static ISpigotBuilder AddAfterReceive(this ISpigotBuilder src, Action<CloudEvent> afterReceiveAction)
        {
            src.AfterReceive = afterReceiveAction;
            return src;
        }

        /// <summary>
        /// Assigns a friendly name to the Spigot
        /// </summary>
        /// <param name="src"></param>
        /// <param name="friendlyName"></param>
        /// <returns></returns>
        public static ISpigotBuilder WithFriendlyName(this ISpigotBuilder src, string friendlyName)
        {
            src.ApplicationName = friendlyName;
            return src;
        }

        /// <summary>
        /// Registers a <see cref="Knob"/> to handle messages from Spigot Streams
        /// </summary>
        /// <param name="src"></param>
        /// <typeparam name="TKnob"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static ISpigotBuilder AddKnob<TKnob, T>(this ISpigotBuilder src) where TKnob : Knob<T> where T : class, new()
        {
            src.Services.AddSingleton<TKnob>();
            src.Knobs.Add(typeof(TKnob));
            return src;
        }

        /// <summary>
        /// Registers a <see cref="Knob"/> to handle messages from Spigot Streams
        /// </summary>
        /// <param name="src"></param>
        /// <param name="instance"></param>
        /// <typeparam name="TKnob"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static ISpigotBuilder AddKnob<TKnob, T>(this ISpigotBuilder src, TKnob instance) where TKnob : Knob<T> where T : class, new()
        {
            src.Services.AddSingleton(instance);
            src.Knobs.Add(typeof(TKnob));
            return src;
        }
    }
}