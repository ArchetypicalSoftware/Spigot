using System;
using System.Collections.Generic;
using CloudNative.CloudEvents;
using CloudNative.CloudEvents.Extensions;

namespace Archetypical.Software.Spigot
{
    /// <summary>
    /// The internal event that is raised and the envelope for the message serialized
    /// </summary>
    public class Envelope : CloudEvent
    {
        public Envelope() : base(CloudEventsSpecVersion.Default, new ICloudEventExtension[]
        {
            new SamplingExtension(),
            new DistributedTracingExtension(),
            new IntegerSequenceExtension()
        })
        {
        }
    }
}