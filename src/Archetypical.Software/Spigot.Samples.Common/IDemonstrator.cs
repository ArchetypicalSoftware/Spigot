using System.IO;
using System.Threading.Tasks;

namespace Spigot.Samples.Common
{
    public interface IDemonstrator
    {
        Task GoAsync(TextWriter writer);

        void Describe(TextWriter writer);
    }
}