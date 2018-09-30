using Archetypical.Software.Spigot;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace Spigot.LoadTests
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<Spigots>();
        }
    }

    [ClrJob, CoreJob, SimpleJob(baseline: true)]
    [RankColumn]
    public class Spigots
    {
        [Benchmark()]
        public void SendTest() => Spigot<MyTestClass>.Send(new MyTestClass());
    }

    public class MyTestClass
    {
        public int Test { get; set; }
    }
}