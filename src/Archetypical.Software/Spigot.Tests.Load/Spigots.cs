using Archetypical.Software.Spigot;
using Archetypical.Software.Spigot.Extensions;
using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.Logging;

namespace Spigot.Tests.Load
{
    [SimpleJob(runtimeMoniker: BenchmarkDotNet.Jobs.RuntimeMoniker.NetCoreApp31)]
    [SimpleJob(runtimeMoniker: BenchmarkDotNet.Jobs.RuntimeMoniker.Net472)]
    [RankColumn, AllStatisticsColumn, IterationsColumn]
    [MemoryDiagnoser]
    [MarkdownExporter, HtmlExporter, XmlExporter, RPlotExporter]
    public class Spigots
    {
        public Spigots()
        {
            var services = new ServiceCollection();
            var config = new ConfigurationBuilder().Build();
            cde = new CountdownEvent(10000);
            services
                .AddSingleton(cde)
                .AddLogging()
                .AddSpigot(config)
                .AddKnob<SignalingHandler, MyTestClass>()
                .Build();
            serviceProvider = services.BuildServiceProvider();
        }

        private CountdownEvent cde;
        private ServiceProvider serviceProvider;

        //[Params(1, 10, 20, 30, 40, 50, 100, 250, 500, 1000, 5000, 10000)]
        //public int Iterations { get; set; } = 100;

        [Benchmark]
        public int SendOne()
        {
            var sender = serviceProvider.GetService<MessageSender<MyTestClass>>();
            sender.Send(new MyTestClass());
            cde.Wait(TimeSpan.FromMilliseconds(10));
            return cde.CurrentCount;
        }

        [Benchmark]
        public int SendTen()
        {
            var sender = serviceProvider.GetService<MessageSender<MyTestClass>>();
            for (int i = 0; i < 10; i++)
            {
                sender.Send(new MyTestClass());
            }
            cde.Wait(TimeSpan.FromMilliseconds(10));
            return cde.CurrentCount;
        }

        [Benchmark(Baseline = true)]
        public int SendOneHundred()
        {
            var sender = serviceProvider.GetService<MessageSender<MyTestClass>>();
            for (int i = 0; i < 100; i++)
            {
                sender.Send(new MyTestClass());
            }
            cde.Wait(TimeSpan.FromMilliseconds(100));
            return cde.CurrentCount;
        }

        [Benchmark]
        public int SendThousand()
        {
            var sender = serviceProvider.GetService<MessageSender<MyTestClass>>();
            for (int i = 0; i < 1000; i++)
            {
                sender.Send(new MyTestClass());
            }
            cde.Wait(TimeSpan.FromMilliseconds(1000));
            return cde.CurrentCount;
        }

        [Benchmark]
        public int SendTenThousand()
        {
            var sender = serviceProvider.GetService<MessageSender<MyTestClass>>();
            for (int i = 0; i < 10000; i++)
            {
                sender.Send(new MyTestClass());
            }
            cde.Wait(TimeSpan.FromMilliseconds(10000));
            return cde.CurrentCount;
        }
    }

    public class SignalingHandler : Knob<MyTestClass>
    {
        private readonly CountdownEvent _cde;

        public SignalingHandler(CountdownEvent cde, Archetypical.Software.Spigot.Spigot spigot, ILogger<Knob<MyTestClass>> logger) : base(spigot, logger)
        {
            _cde = cde;
        }

        protected override void HandleMessage(EventArrived<MyTestClass> message)
        {
            _cde.Signal();
        }
    }
}