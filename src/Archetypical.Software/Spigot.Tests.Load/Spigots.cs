using Archetypical.Software.Spigot;
using BenchmarkDotNet.Attributes;
using System;
using System.Threading;
using System.Threading.Tasks.Dataflow;

namespace Spigot.Tests.Load
{
    [ClrJob, CoreJob]
    [RankColumn, AllStatisticsColumn, IterationsColumn]
    [MemoryDiagnoser]
    [MarkdownExporter, HtmlExporter, RPlotExporter, XmlExporter]
    public class Spigots
    {
        [Params(1, 10, 20, 30, 40, 50, 100, 500, 1000, 10000)]
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
            var block = new ActionBlock<int>(i => Spigot<MyTestClass>.Send(new MyTestClass() { Test = i }));

            for (int i = 0; i < Iterations; i++)
            {
                block.Post(i);
            }

            block.Complete();

            block.Completion.GetAwaiter().GetResult();

            cde.Wait(TimeSpan.FromMilliseconds(10 * Iterations));
            Spigot<MyTestClass>.Open -= OnSpigotOnOpen;
            return cde.CurrentCount;
        }
    }
}