using System.IO;

namespace Spigot.Samples
{
    public interface IDemonstrator
    {
        void Go(TextWriter writer);

        void Describe(TextWriter writer);
    }
}