using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Archetypical.Software.Spigot.Extensions
{
    public interface ISpigotBuilder
    {
        IServiceCollection Services { get; set; }
        IConfiguration Configuration { get; set; }
        SpigotSettings Settings { get; set; }
        ISpigotSerializer SerializerFactory { get; set; }
        ISpigotStream StreamFactory { get; set; }
        ILoggerFactory LoggerFactory { get; set; }

        void Build();
    }
}