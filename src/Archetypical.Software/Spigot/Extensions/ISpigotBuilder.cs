using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Archetypical.Software.Spigot.Extensions
{
    /// <summary>
    /// Represents a builder object that controls how a Spigot is configured
    /// </summary>
    public interface ISpigotBuilder
    {
        /// <summary>
        /// The services collection for storing instances and settings
        /// </summary>
        IServiceCollection Services { get; set; }

        /// <summary>
        /// Configuration settings
        /// </summary>
        IConfiguration Configuration { get; set; }

        /// <summary>
        /// The name of the current application.
        /// </summary>
        /// <remarks>Set this to indicate which business-friendly product is hosting these spigots</remarks>
        /// <example>Inventory Service, Billing Service, Monitoring Application</example>
        string ApplicationName { get; set; }

        /// <summary>
        /// The unique identifier of this instance.
        /// </summary>
        Guid InstanceIdentifier { get; }

        /// <summary>
        /// The delegate to call before sending data to be serialized by the <see cref="ISpigotSerializer"/> to be sent on the <see cref="ISpigotStream"/>
        /// </summary>
        /// <remarks>This will not provide access to the data but only to the <see cref="Envelope"/> This should only manipulate the <see cref="Headers"/></remarks>
        Action<Envelope> BeforeSend { get; set; }

        /// <summary>
        /// The delegate to call after receiving the <see cref="Envelope"/> but before sending the data to be deserialized by the <see cref="ISpigotSerializer"/>
        /// </summary>
        /// <remarks>This will not provide access to the data but only to the <see cref="Envelope"/> This should only manipulate the <see cref="Headers"/></remarks>
        Action<Envelope> AfterReceive { get; set; }

        Resilience Resilience { get; set; }

        /// <summary>
        /// This method will execute all late bound actions and prepare the Spigot for uses
        /// </summary>
        void Build();

        /// <summary>
        /// Knob Types
        /// </summary>
        List<Type> Knobs { get; set; }
    }
}