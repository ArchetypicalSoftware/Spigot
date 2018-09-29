using System;
using Archetypical.Software.Spigot;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;


namespace Spigot.LoadTests
{
    class Program
    {
        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<Spigots>();

        }
    }

    [ClrJob(true),CoreJob,DryCoreJob,DryCoreRtJob,DryClrJob,LegacyJitX64Job,SimpleJob]
    [RankColumn]
    public class Spigots
    {
        [Benchmark]
        public void SendTest() => Spigot<MyTestClass>.Send(new MyTestClass());
    }

    public class MyTestClass
    {
        public int Test { get; set; }
    }
}
