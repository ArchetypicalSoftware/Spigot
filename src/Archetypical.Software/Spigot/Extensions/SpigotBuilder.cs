using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Archetypical.Software.Spigot.Extensions
{
    internal class SpigotBuilder : ISpigotBuilder
    {
        public IServiceCollection Services { get; set; }
        public IConfiguration Configuration { get; set; }
        public SpigotSettings Settings { get; set; } = new SpigotSettings();
        public ISpigotSerializer SerializerFactory { get; set; } = new DefaultDataContractSerializer();
        public ISpigotStream StreamFactory { get; set; } = new LocalStream();
        public ILoggerFactory LoggerFactory { get; set; }

        public void Build()
        {
            Spigot.Setup(this);
        }
    }
}