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

        [Benchmark]
        public void Serialize()
        {
            var bytes = Archetypical.Software.Spigot.Spigot.Settings.SerializerFactory().Serialize<MyTestClass>(new MyTestClass()
            {
                Test = int.MaxValue
            });
        
            var copy = Archetypical.Software.Spigot.Spigot.Settings.SerializerFactory().Deserialize<MyTestClass>(bytes);
        }
    }

    internal class Program
    {
        private static void Main(string[] args)
        {
            BenchmarkRunner.Run<Spigots>();
        }
    }
}