using System;
using CloudNative.CloudEvents;
using CloudNative.CloudEvents.Extensions;
using Microsoft.Extensions.Logging;

namespace Archetypical.Software.Spigot
{
    /// <summary>
    /// Generic class that allows the sending of typed data to a stream
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MessageSender<T> where T : class, new()
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
        public void Send(T eventData)
        {
            var wrapper = new CloudEvent(CloudEventsSpecVersion.Default, new ICloudEventExtension[]
            {
                new SamplingExtension(),
                new DistributedTracingExtension(),
                new IntegerSequenceExtension()
            })
            {
                Data = _spigot.Serializer.Serialize(eventData),
                DataContentType = _spigot.Serializer.ContentType,
                Type = typeof(T).Name,
                Subject = typeof(T).FullName,
                Source = new Uri
                (
                    $"urn:spigot-{Environment.CurrentManagedThreadId}:{_spigot.ApplicationName}:{_spigot.InstanceIdentifier}"
                )
            };
            _spigot.BeforeSend?.Invoke(wrapper);
            _logger.LogTrace("Sending [{0}] with id {1}", wrapper.Type, wrapper.Id);

            //Send it to all listeners in the same process space
            if (_spigot.Knobs.ContainsKey(typeof(T).Name))
                _spigot.Knobs[typeof(T).Name]?.Invoke(wrapper);

            var contents = new CloudEventContent(wrapper, ContentMode.Structured, new JsonEventFormatter());
            var bytes = contents.ReadAsByteArrayAsync().GetAwaiter().GetResult();

            foreach (var stream in _spigot.Streams)
                _spigot.Resilience.Sending.Execute(() =>
                {
                    _logger.LogTrace("Sending using resilience.");
                    var result = stream?.TrySend(bytes);
                    if (!result.GetValueOrDefault())
                    {
                        throw new Exception("Sending exception");
                    }
                });
        }
    }
}