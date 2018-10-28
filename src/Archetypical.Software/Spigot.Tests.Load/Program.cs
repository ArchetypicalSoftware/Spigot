using System;
using BenchmarkDotNet.Running;

namespace Spigot.LoadTests
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<Spigots>();
            var x = summary;
            Console.ReadKey();
        }
    }
}