using System;
using System.Linq;
using System.Threading;
using Archetypical.Software.Spigot;
using BenchmarkDotNet.Attributes;

namespace Spigot.LoadTests
{
    [ClrJob]
    [RankColumn, AllStatisticsColumn, IterationsColumn]
    [MemoryDiagnoser]
    [MarkdownExporter, HtmlExporter, RPlotExporter, XmlExporter]
    public class Spigots
    {
        [Params(1, 10, 20, 30, 40, 50, 100)]
        public int Iterations { get; set; } = 100;

        [Benchmark]
        public int SendTest()
        {
            var cde = new CountdownEvent(Iterations);

            void OnSpigotOnOpen(object sender, EventArrived<MyTestClass> e)
            {
                cde.Signal();
            }

            Spigot<MyTestClass>.Open += OnSpigotOnOpen;
            Enumerable.Range(0, Iterations).AsParallel().ForAll(i => Spigot<MyTestClass>.Send(new MyTestClass()));

            cde.Wait(TimeSpan.FromMilliseconds(10 * Iterations));
            Spigot<MyTestClass>.Open -= OnSpigotOnOpen;
            return cde.CurrentCount;
        }
    }
}