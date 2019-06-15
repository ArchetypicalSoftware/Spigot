using BenchmarkDotNet.Running;
using System;

namespace Spigot.Tests.Load
{
    public class Program
    {
        private static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<Spigots>();
            Console.WriteLine(summary.AllRuntimes);
            Console.WriteLine(summary.ResultsDirectoryPath);
            Console.ReadKey();
        }
    }
}