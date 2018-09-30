using Archetypical.Software.Spigot;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace Spigot.LoadTests
{
    public class MyTestClass
    {
        public int Test { get; set; }
    }

    [ClrJob, CoreJob, SimpleJob(baseline: true)]
    [RankColumn, AllStatisticsColumn]
    [MarkdownExporter, AsciiDocExporter, HtmlExporter, CsvExporter, RPlotExporter, XmlExporter]
    public class Spigots
    {
        [Benchmark()]
        public void SendTest() => Spigot<MyTestClass>.Send(new MyTestClass());
    }

    internal class Program
    {
        private static void Main(string[] args)
        {
            BenchmarkRunner.Run<Spigots>();
        }
    }
}