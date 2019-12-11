using System;
using Microsoft.Extensions.Logging;
using Polly;

namespace Archetypical.Software.Spigot
{
    /// <summary>
    /// A collection of settings that control the Spigots in this domain
    /// </summary>
    public class SpigotSettings
    {
        
        /// <inheritdoc />
        public SpigotSettings()
        {
            Resilience = new Resilience();
             
            ApplicationName = Environment.CommandLine;
        }



        /// <summary>
        /// The name of the current application.
        /// </summary>
        /// <remarks>Set this to indicate which business-friendly product is hosting these spigots</remarks>
        /// <example>Inventory Service, Billing Service, Monitoring Application</example>
        public string ApplicationName { get; set; }

        /// <summary>
        /// The unique identifier of this instance. 
        /// </summary>
        public Guid InstanceIdentifier { get; } = Guid.NewGuid();

        /// <summary>
        /// The delegate to call before sending data to be serialized by the <see cref="ISpigotSerializer"/> to be sent on the <see cref="ISpigotStream"/>
        /// </summary>
        /// <remarks>This will not provide access to the data but only to the <see cref="Envelope"/> This should only manipulate the <see cref="Headers"/></remarks>
        public Action<Envelope> BeforeSend { get; set; }

        /// <summary>
        /// The delegate to call after receiving the <see cref="Envelope"/> but before sending the data to be deserialized by the <see cref="ISpigotSerializer"/> 
        /// </summary>
        /// <remarks>This will not provide access to the data but only to the <see cref="Envelope"/> This should only manipulate the <see cref="Headers"/></remarks>
        public Action<Envelope> AfterReceive { get; set; }

        public Resilience Resilience { get; set; }
    }

    public class Resilience
    {
        public Resilience()
        {
            Sending = new Retry();
            Receiving = new Retry();
            Lifetimes = new Lifetime();
        }
        public class Retry
        {
            public int RetryAttempts { get; set; }=3;
            public TimeSpan TimeToWaitBetweenAttempts { get; set; } = TimeSpan.FromSeconds(1);

            public void Execute(Action action)
            {
                Policy.Handle<Exception>().WaitAndRetry(RetryAttempts,
                    i => TimeToWaitBetweenAttempts, (e, t) =>
                        {
                            Console.WriteLine($"Failed with {e}.Reattempting in {t}");
                        })
                    .Execute(action);
            }
        }

        public class Lifetime
        {
            public TimeSpan MessageValidFor { get; set; } = TimeSpan.FromMinutes(1);
        }

        public Retry Sending { get; set; }

        public Retry Receiving { get; set; }

        public Lifetime Lifetimes { get; set; }

    }
}