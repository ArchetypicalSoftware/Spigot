namespace Archetypical.Software.Spigot
{
    public interface ISpigotSerializer{

        byte[] Serialize<T>(T dataToSerialize) where T : class, new();
        T Deserialize<T>(byte[] serializedByteArray) where T : class, new();
    }
}