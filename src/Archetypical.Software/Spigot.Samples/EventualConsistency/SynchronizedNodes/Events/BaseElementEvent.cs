namespace Spigot.Samples.EventualConsistency.SynchronizedNodes.Events
{
    /// <summary>
    /// This class is only necessary for the demonstration since many events are beings raised from the same appdomain and not from different instances.
    /// If this were a real world scenario, this can be done in the "BeforeCall" action on the SpigotSettings
    /// </summary>
    public abstract class BaseElementEvent
    {
        public string Instance { get; set; }
    }
}