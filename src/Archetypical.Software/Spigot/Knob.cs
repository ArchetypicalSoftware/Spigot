using System;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace Archetypical.Software.Spigot
{
    public abstract class Knob<T> : Knob where T : class, new()
    {
        private readonly ILogger<Knob<T>> _logger;

        protected Knob(Spigot spigot, ILogger<Knob<T>> logger) : base(spigot, logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Gets invoked whenever an instance of T is received from the <see cref="ISpigotStream"/>
        /// </summary>
        protected abstract void HandleMessage(EventArrived<T> message);

        public override void HandleMessage(Envelope message)
        {
            if (!(message is Envelope arrived)) return;
            _logger.LogTrace("Envelope of type [{0}] arrived with id {1}", arrived.Event, arrived.MessageIdentifier);
            Dispatch(arrived);
        }

        protected void Dispatch(Envelope e)
        {
            _spigot.AfterReceive?.Invoke(e);
            var message = new EventArrived<T>
            {
                EventData = _spigot.Serializer.Deserialize<T>(e.SerializedEventData),
                Context = new Context
                {
                    Headers = e.Headers,
                    Sender = e.Sender
                }
            };
            _logger.LogTrace($"Received {e.Event} message from stream with id {e.MessageIdentifier}");

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
            _spigot.Register(this);
        }
    }

    public abstract class Knob
    {
        protected readonly Spigot _spigot;
        private readonly ILogger<Knob> _logger;

        protected Knob(Spigot spigot, ILogger<Knob> logger)
        {
            _spigot = spigot;
            _logger = logger;
        }

        protected virtual void Dispatch(Envelope e)
        {
            // Should be overwritten
        }

        public virtual void HandleMessage(Envelope message)
        {
            //Should be overwritten
        }

        public virtual void Register()
        {
            //to be overwritten
        }
    }
}