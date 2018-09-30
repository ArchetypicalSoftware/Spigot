namespace Archetypical.Software.Spigot
{
    /// <summary>
    /// Allows for the serialization and deserialization of types.
    /// Each implementation should handle its own optimization and fault tolerance.
    /// </summary>
    /// <remarks>Types of T may not be available at compile-time so special handling will need to be done for any attribute based configuration.</remarks>
    public interface ISpigotSerializer
    {
        /// <summary>
        /// Serializes the class to a byte array 
        /// </summary>
        /// <typeparam name="T">The Type to be serialized. Must be a class and have a parameter-less constructor.</typeparam>
        /// <param name="dataToSerialize">The instance of the class to serialize</param>
        /// <returns></returns>
        byte[] Serialize<T>(T dataToSerialize) where T : class, new();

        /// <summary>
        /// Deserializes the raw byte array back to an instance of the class
        /// </summary>
        /// <typeparam name="T">The Type to be serialized. Must be a class and have a parameter-less constructor.</typeparam>
        /// <param name="serializedByteArray">The raw bytes received from the  <see cref="ISpigotStream"/> to be deserialized</param>
        /// <returns></returns>
        T Deserialize<T>(byte[] serializedByteArray) where T : class, new();
    }
}