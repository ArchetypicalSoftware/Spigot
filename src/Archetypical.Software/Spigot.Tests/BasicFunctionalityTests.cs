using Archetypical.Software.Spigot;
using Archetypical.Software.Spigot.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using Xunit;
using Xunit.Abstractions;

namespace Spigot.Tests
{
    public class BasicFunctionalityTests
    {
        private readonly ILoggerFactory _factory;

        private readonly IConfiguration _config;
        private const string Expected = "Pre-sending Header Value";

        public BasicFunctionalityTests(ITestOutputHelper outputHelper)
        {
            _factory = new LoggerFactory();
            _factory.AddProvider(new XunitLoggerProvider(outputHelper));
            _config = new ConfigurationBuilder().Build();
        }

        [Fact]
        public void WorksOutOfTheBox()
        {
            var testNumber = 100;
            var counter = new EventNumber(testNumber);
            var services = new ServiceCollection();
            services
                .AddLogging()
                .AddSingleton(counter)
            .AddSpigot(_config)
            .AddKnob<SimpleClass1Handler, SimpleClass1>()
            .AddKnob<ComplexClassHandler, ComplexClass>()
                .Build();
            var provider = services.BuildServiceProvider();

            var simpleSender = provider.GetService<MessageSender<SimpleClass1>>();
            var complexSender = provider.GetService<MessageSender<ComplexClass>>();

            simpleSender.Send(new SimpleClass1(testNumber));
            simpleSender.Send(new SimpleClass1(testNumber));
            complexSender.Send(new ComplexClass(testNumber));
            Assert.Equal(3, counter.EventsReceived);
        }

        //[Fact]
        //public void Multiple_Handlers_Get_Called_Even_When_One_Errors()
        //{
        //    int eventsReceived = 0;
        //    var testNumber = 400;

        //    void OnSpigotOnOpen(object s, EventArrived<SimpleClass1> e) => throw new Exception();
        //    void OnOpen(object s, EventArrived<SimpleClass1> e)
        //    {
        //        if (e.EventData.Index == testNumber) eventsReceived++;
        //    }
        //    Spigot<SimpleClass1>.Open += OnSpigotOnOpen;
        //    Spigot<SimpleClass1>.Open += OnOpen;
        //    Spigot<SimpleClass1>.Send(new SimpleClass1(testNumber));
        //    Spigot<SimpleClass1>.Open -= OnSpigotOnOpen;
        //    Spigot<SimpleClass1>.Open -= OnOpen;

        //    Assert.Equal(1, eventsReceived);
        //}

        [Fact]
        public void Routes_Correctly()
        {
            var testNumber = 100;
            var counter = new EventNumber(testNumber);
            var services = new ServiceCollection();
            services
                .AddLogging()
                .AddSingleton(counter)
                .AddSpigot(_config)
                .AddKnob<SimpleClass1Handler, SimpleClass1>()
                .AddKnob<ComplexClassHandler, ComplexClass>()
                .Build();
            var provider = services.BuildServiceProvider();

            var simpleSender = provider.GetService<MessageSender<SimpleClass1>>();
            var complexSender = provider.GetService<MessageSender<ComplexClass>>();

            simpleSender.Send(new SimpleClass1(testNumber));
            simpleSender.Send(new SimpleClass1(testNumber));
            Assert.Equal(2, counter.EventsReceived);
            complexSender.Send(new ComplexClass(testNumber));
            Assert.Equal(3, counter.EventsReceived);
        }

        [Fact]
        public void Pre_And_Post_Handlers_Work_As_Expected()
        {
            var testNumber = 600;

            var counter = new EventNumber(testNumber, Expected);
            var services = new ServiceCollection();
            services
                .AddLogging()
                .AddSingleton(counter)
                .AddSpigot(_config)
                .AddKnob<SimpleHeaderValidator, SimpleClass1>()
                .AddBeforeSend(env =>
                {
                    env.GetAttributes().Add("Test", Expected);
                })
                .Build();
            var provider = services.BuildServiceProvider();

            var simpleSender = provider.GetService<MessageSender<SimpleClass1>>();

            simpleSender.Send(new SimpleClass1(testNumber));
            simpleSender.Send(new SimpleClass1(testNumber));
            Assert.Equal(2, counter.EventsReceived);
        }

        [Fact]
        public void Correct_number_of_iterations()
        {
            var cde = new CountdownEvent(100);
            var testNumber = 600;

            var counter = new EventNumber(testNumber, "Test");
            var services = new ServiceCollection();
            services
                .AddLogging()
                .AddSingleton(counter)
                .AddSingleton(cde)
                .AddSpigot(_config)
                .AddKnob<SignalingHandler, SimpleClass1>()
                .Build();
            var provider = services.BuildServiceProvider();

            var simpleSender = provider.GetService<MessageSender<SimpleClass1>>();

            for (int i = 0; i < 100; i++)
                simpleSender.Send(new SimpleClass1(testNumber));

            cde.Wait(TimeSpan.FromMilliseconds(10 * 100));
            Assert.Equal(0, cde.CurrentCount);
        }

        //[Fact]
        //public void Serialization_And_Deserialization_Work_As_Expected()
        //{
        //    var expected = "Serialized Values";

        //    var testNumber = 700;
        //    var complexClass = new ComplexClass(testNumber);

        //    void OnSpigotOnOpen(object s, EventArrived<ComplexClass> e)
        //    {
        //        if (e.EventData.Index == testNumber) Assert.Equal(complexClass.GetHashCode(), e.EventData.GetHashCode());
        //    }

        //    Spigot<ComplexClass>.Open += OnSpigotOnOpen;
        //    Spigot<ComplexClass>.Send(complexClass);
        //    Spigot<ComplexClass>.Open -= OnSpigotOnOpen;
        //}

        public class SimpleClass1
        {
            public SimpleClass1() : this(-1)
            {
            }

            public SimpleClass1(int index)
            {
                Index = index;
                Guid = Guid.NewGuid();
                DateTimeOffset = DateTimeOffset.Now;
            }

            public Guid Guid { get; set; }
            public DateTimeOffset DateTimeOffset { get; set; }
            public int Index { get; set; }
        }

        public class ComplexClass
        {
            public ComplexClass() : this(-1)
            {
            }

            public ComplexClass(int index)
            {
                Index = index;
                SimpleClass1 = new SimpleClass1(index)
                {
                };
                Guid = Guid.NewGuid();
                DateTimeOffset = DateTimeOffset.UtcNow;
            }

            public int Index { get; set; }
            public SimpleClass1 SimpleClass1 { get; set; }
            public Guid Guid { get; set; }
            public DateTimeOffset DateTimeOffset { get; set; }
        }

        public class EventNumber
        {
            public string ExpectedHeader { get; set; }

            public EventNumber(int eventNumber, string expectedHeader = "")
            {
                ExpectedHeader = expectedHeader;
                Number = eventNumber;
            }

            public int EventsReceived { get; set; } = 0;
            public int Number { get; set; }
        }

        public class SimpleClass1Handler : Knob<SimpleClass1>
        {
            public SimpleClass1Handler(EventNumber number, Archetypical.Software.Spigot.Spigot spigot, ILogger<Knob<SimpleClass1>> logger) : base(spigot, logger)
            {
                TestNumber = number;
            }

            protected override void HandleMessage(EventArrived<SimpleClass1> message)
            {
                if (message.EventData.Index == TestNumber.Number)
                {
                    TestNumber.EventsReceived++;
                }
            }

            public EventNumber TestNumber { get; set; }
        }

        public class ComplexClassHandler : Knob<ComplexClass>
        {
            public EventNumber TestNumber { get; set; }

            public ComplexClassHandler(EventNumber number, Archetypical.Software.Spigot.Spigot spigot, ILogger<Knob<ComplexClass>> logger) : base(spigot, logger)
            {
                TestNumber = number;
            }

            protected override void HandleMessage(EventArrived<ComplexClass> message)
            {
                if (message.EventData.Index == TestNumber.Number)
                {
                    TestNumber.EventsReceived++;
                }
            }
        }

        public class SimpleHeaderValidator : Knob<SimpleClass1>
        {
            private readonly EventNumber _testnumber;

            public SimpleHeaderValidator(EventNumber testnumber, Archetypical.Software.Spigot.Spigot spigot, ILogger<Knob<SimpleClass1>> logger) : base(spigot, logger)
            {
                _testnumber = testnumber;
            }

            protected override void HandleMessage(EventArrived<SimpleClass1> message)
            {
                if (message.EventData.Index == _testnumber.Number && message.Context.Headers["Test"].ToString() == _testnumber.ExpectedHeader)
                {
                    _testnumber.EventsReceived++;
                }
            }
        }

        public class SignalingHandler : Knob<SimpleClass1>
        {
            public CountdownEvent Cde { get; }

            public SignalingHandler(CountdownEvent cde, Archetypical.Software.Spigot.Spigot spigot, ILogger<Knob<SimpleClass1>> logger) : base(spigot, logger)
            {
                Cde = cde;
            }

            protected override void HandleMessage(EventArrived<SimpleClass1> message)
            {
                Cde.Signal();
            }
        }
    }
}