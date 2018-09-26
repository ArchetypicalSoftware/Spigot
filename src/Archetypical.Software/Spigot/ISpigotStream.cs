namespace Archetypical.Software.Spigot
{
    public interface ISpigotStream
    {

        bool TrySend(byte[] data);

        byte[] MessageCallback();

    }
}