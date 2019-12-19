namespace Archetypical.Software.Spigot
{
    /// <summary>
    /// The message being sent along with the <see cref="Context"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EventArrived<T>
    {
        /// <summary>
        /// The value from the sender
        /// </summary>
        public T EventData { get; set; }

        /// <summary>
        /// The Senders <see cref="Context"/>
        /// </summary>
        public Context Context { get; set; }
    }
}