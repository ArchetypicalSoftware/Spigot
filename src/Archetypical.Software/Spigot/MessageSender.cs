using System;
using Microsoft.Extensions.Logging;

namespace Archetypical.Software.Spigot
{
    public class MessageSender<T>
    {
        private readonly Spigot _spigot;
        private readonly ILogger<MessageSender<T>> _logger;

        public MessageSender(Spigot spigot, ILogger<MessageSender<T>> logger)
        {
            _spigot = spigot;
            _logger = logger;
        }

        /// <summary>
        /// Allows you to send an instance of T to the <see cref="ISpigotStream"/>
        /// </summary>
        /// <param name="eventData">The data to be sent over</param>
        public void Send<T>(T eventData) where T : class, new()
        {
            var wrapper = new Envelope
            {
                SerializedEventData = _spigot.Serializer.Serialize(eventData),
                Event = typeof(T).Name,
                FQN = typeof(T).FullName,
                MessageIdentifier = Guid.NewGuid(),
                Sender = new Sender
                {
                    ProcessId = Environment.CurrentManagedThreadId,
                    Name = _spigot.ApplicationName,
                    InstanceIdentifier = _spigot.InstanceIdentifier
                }
            };
            _spigot.BeforeSend?.Invoke(wrapper);
            _logger.LogTrace("Sending [{0}] with id {1}", wrapper.Event, wrapper.MessageIdentifier);

            //Send it to all listeners in the same process space
            _spigot.Knobs[typeof(T).Name]?.Invoke(wrapper);

            var bytes = _spigot.Serializer.Serialize(wrapper);

            _spigot.Resilience.Sending.Execute(() =>
            {
                _logger.LogTrace("Sending using resilience.");
                var result = _spigot.Stream?.TrySend(bytes);
                if (!result.GetValueOrDefault())
                {
                    throw new Exception("Sending exception");
                }
            });
        }
    }
}