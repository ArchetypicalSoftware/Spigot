using System;
using System.Linq;
using CloudNative.CloudEvents;
using Microsoft.Extensions.Logging;

namespace Archetypical.Software.Spigot
{
    /// <summary>
    /// Creates a strongly typed-knob that implements a controller pattern for events
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Knob<T> : Knob where T : class, new()
    {
        private readonly ILogger<Knob<T>> _logger;

        /// <summary>
        /// the base constructor. Pass in your typed loggers here
        /// </summary>
        /// <param name="spigot"></param>
        /// <param name="logger"></param>
        protected Knob(Spigot spigot, ILogger<Knob<T>> logger) : base(spigot)
        {
            _logger = logger;
        }

        /// <summary>
        /// Gets invoked whenever an instance of T is received from the <see cref="ISpigotStream"/>
        /// </summary>
        protected abstract void HandleMessage(EventArrived<T> message);

        /// <inheritdoc />
        public override void HandleMessage(CloudEvent arrived)
        {
            _logger.LogTrace("Envelope of type [{0}] arrived with id {1}", arrived.Type, arrived.Id);
            Dispatch(arrived);
        }

        /// <inheritdoc />
        protected override void Dispatch(CloudEvent e)
        {
            _logger.LogTrace("dispatching...");
            Spigot.AfterReceive?.Invoke(e);
            var message = new EventArrived<T>
            {
                EventData = Spigot.Serializer.Deserialize<T>(e.Data as byte[]),
                Context = new Context
                {
                    Headers = e.GetAttributes(),
                    Sender = new Sender(e.Source)
                }
            };
            _logger.LogTrace($"Received {e.Type} message from stream with id {e.Id}");

            try
            {
                HandleMessage(message);
            }
            catch (Exception exx)
            {
                _logger.LogCritical(exx, $"User code threw an exception.");
            }
        }

        public override void Register()
        {
            Spigot.Register(this);
        }
    }

    public abstract class Knob
    {
        protected readonly Spigot Spigot;

        protected Knob(Spigot spigot)
        {
            Spigot = spigot;
        }

        /// <summary>
        /// Dispatches a <see cref="CloudEvent"/> to the registered handler
        /// </summary>
        /// <param name="e"></param>
        /// <exception cref="InvalidOperationException"></exception>
        protected virtual void Dispatch(CloudEvent e)
        {
            // Should be overwritten
            throw new InvalidOperationException("This method should be implemented in a higher class");
        }

        /// <summary>
        /// Internal handler of the raw <see cref="CloudEvent"/>
        /// </summary>
        /// <param name="message"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public virtual void HandleMessage(CloudEvent message)
        {
            //Should be overwritten
            throw new InvalidOperationException("This method should be implemented in a higher class");
        }

        /// <summary>
        /// Registers a know into the service collection
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public virtual void Register()
        {
            //to be overwritten
            throw new InvalidOperationException("This method should be implemented in a higher class");
        }
    }
}