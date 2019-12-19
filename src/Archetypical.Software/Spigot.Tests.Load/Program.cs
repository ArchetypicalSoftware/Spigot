using BenchmarkDotNet.Running;
using System;

namespace Spigot.Tests.Load
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<Spigots>();
            var x = summary;
        }
    }
}