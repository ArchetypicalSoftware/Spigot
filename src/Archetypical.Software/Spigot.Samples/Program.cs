using System;

namespace Spigot.Samples
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine(@"
 _____         _                _
/  ___|       (_)              | |
\ `--.  _ __   _   __ _   ___  | |_
 `--. \| '_ \ | | / _` | / _ \ | __|
/\__/ /| |_) || || (_| || (_) || |_
\____/ | .__/ |_| \__, | \___/  \__|
       | |         __/ |
       |_|        |___/

=======================================
");
            IDemonstrator demonstrator = new EventualConsistency.MaterializedView.Demonstrator();
            demonstrator.Describe(Console.Out);
            demonstrator.Go(Console.Out);
            Console.ReadKey();
        }
    }
}