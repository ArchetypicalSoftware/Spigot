using BenchmarkDotNet.Running;
using System;

namespace Spigot.Tests.Load
{
    internal class Program
    {
        private static void Main()
        {
            BenchmarkRunner.Run<Spigots>();
        }
    }
}