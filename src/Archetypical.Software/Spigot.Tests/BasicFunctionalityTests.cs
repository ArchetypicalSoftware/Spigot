using System;
using System.Diagnostics;
using System.Threading;
using Archetypical.Software.Spigot;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;

namespace Spigot.Tests
{
    class XunitLoggerProvider :ILoggerProvider
    {
        private ITestOutputHelper _outputHelper;
        public XunitLoggerProvider(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
        }
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new XunitLogger(_outputHelper);
        }
    }

    class XunitLogger: ILogger
    {
        private ITestOutputHelper _outputHelper;
        public XunitLogger(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            _outputHelper.WriteLine($"{logLevel}:Thread:{Thread.CurrentThread.ManagedThreadId} - {formatter(state, exception)}");
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true
                ;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            throw new NotImplementedException();
        }
    }

public class BasicFunctionalityTests
    {
        ILoggerFactory factory;
        public BasicFunctionalityTests(ITestOutputHelper outputHelper)
        {
            factory = new LoggerFactory();
                factory.AddProvider(new XunitLoggerProvider(outputHelper));

            Debug.Listeners.Add(new DefaultTraceListener());
            Archetypical.Software.Spigot.Spigot.Setup(settings =>
            {
                settings.AddLoggerFactory(factory);
            });
        }

        [Fact]
        public void WorksOutOfTheBox()
        {
            var eventsReceived = 0;
            var testNumber = 100;
            void FirstHandler(object s, EventArrived<SimpleClass1> e)
            {
                if(e.EventData.Index == testNumber ) eventsReceived++;
            }
            void SecondHandler(object s, EventArrived<ComplexClass> e)
            {
                if (e.EventData.Index == testNumber) eventsReceived++;
            }

            Spigot<SimpleClass1>.Open += FirstHandler;
            Spigot<ComplexClass>.Open += SecondHandler;
            Console.WriteLine("sending");
            Spigot<SimpleClass1>.Send(new SimpleClass1(testNumber));
            Spigot<SimpleClass1>.Send(new SimpleClass1(testNumber));
            Spigot<ComplexClass>.Send(new ComplexClass(testNumber));
            Spigot<SimpleClass1>.Open -= FirstHandler;
            Spigot<ComplexClass>.Open -= SecondHandler;
            Assert.Equal(3, eventsReceived);
        }

        [Fact]
        public void Multiple_Handlers_Get_Called()
        {
            int eventsReceived = 0; int testNumber = 200;

            void OnSpigotOnOpen(object s, EventArrived<SimpleClass1> e)
            {
                if (e.EventData.Index == testNumber) eventsReceived++;
            }

            Spigot<SimpleClass1>.Open += OnSpigotOnOpen;
            Spigot<SimpleClass1>.Open += OnSpigotOnOpen;
            Spigot<SimpleClass1>.Send(new SimpleClass1(testNumber));
            Spigot<SimpleClass1>.Open -= OnSpigotOnOpen;
            Spigot<SimpleClass1>.Open -= OnSpigotOnOpen;

            Assert.Equal(2, eventsReceived);
        }

        [Fact]
        public void Handlers_Can_Be_Added_And_Removed()
        {
            int eventsReceived = 0;
            var testNumber = 300;
            void FirstHandler(object s, EventArrived<SimpleClass1> e)
            {
                if (e.EventData.Index == testNumber) eventsReceived++;
            }
            void SecondHandler(object s, EventArrived<SimpleClass1> e)
            {
                if (e.EventData.Index == testNumber) eventsReceived++;
            }
            Spigot<SimpleClass1>.Open += FirstHandler;
            Spigot<SimpleClass1>.Open += SecondHandler;
            Spigot<SimpleClass1>.Send(new SimpleClass1(testNumber));
            Spigot<SimpleClass1>.Open -= FirstHandler;

            Spigot<SimpleClass1>.Send(new SimpleClass1(testNumber));
            Spigot<SimpleClass1>.Open -= SecondHandler;
            Assert.Equal(3, eventsReceived);
        }

        [Fact]
        public void Multiple_Handlers_Get_Called_Even_When_One_Errors()
        {
            int eventsReceived = 0;
            var testNumber = 400;

            void OnSpigotOnOpen(object s, EventArrived<SimpleClass1> e) => throw new Exception();
            void OnOpen(object s, EventArrived<SimpleClass1> e)
            {
                if (e.EventData.Index == testNumber) eventsReceived++;
            }
            Spigot<SimpleClass1>.Open += OnSpigotOnOpen;
            Spigot<SimpleClass1>.Open += OnOpen;
            Spigot<SimpleClass1>.Send(new SimpleClass1(testNumber));
            Spigot<SimpleClass1>.Open -= OnSpigotOnOpen;
            Spigot<SimpleClass1>.Open -= OnOpen;

            Assert.Equal(1, eventsReceived);
        }

        [Fact]
        public void Routes_Correctly()
        {
            int eventsClass1Received = 0;
            int eventsClass2Received = 0;
            var testNumber = 500;

            void OnSpigotOnOpen(object s, EventArrived<SimpleClass1> e)
            {
                if (e.EventData.Index == testNumber) eventsClass1Received++;
            }
            
            void OnOpen(object s, EventArrived<ComplexClass> e)
            {
                if (e.EventData.Index == testNumber) eventsClass2Received++;
            }

            Spigot<ComplexClass>.Open += OnOpen;
            Spigot<SimpleClass1>.Open += OnSpigotOnOpen;

            Spigot<SimpleClass1>.Send(new SimpleClass1(testNumber));
            Spigot<SimpleClass1>.Send(new SimpleClass1(testNumber));
            Spigot<ComplexClass>.Send(new ComplexClass(testNumber));

            Spigot<ComplexClass>.Open -= OnOpen;
            Spigot<SimpleClass1>.Open -= OnSpigotOnOpen;


            Assert.Equal(2, eventsClass1Received);
            Assert.Equal(1, eventsClass2Received);
        }


        [Fact]
        public void Pre_And_Post_Handlers_Work_As_Expected()
        {
            var expected = Guid.NewGuid().ToString();
            Archetypical.Software.Spigot.Spigot.Setup(settings => { settings.BeforeSend = env =>
                {
                    env.Headers.Add(new Header("Test",expected));
                };
                
            });
            var testNumber = 600;

            void OnSpigotOnOpen(object s, EventArrived<SimpleClass1> e)
            {
                if (e.EventData.Index == testNumber) Assert.Equal(expected, e.Context.Headers["Test"]?.Value);
            }

            Spigot<SimpleClass1>.Open += OnSpigotOnOpen;
            Spigot<SimpleClass1>.Send(new SimpleClass1(testNumber));
            Spigot<SimpleClass1>.Open -= OnSpigotOnOpen;
        }


        [Fact]
        public void Serialization_And_Deserialization_Work_As_Expected()
        {
            var expected = Guid.NewGuid().ToString();
            Archetypical.Software.Spigot.Spigot.Setup(settings => {
                settings.BeforeSend = env =>
                {
                    env.Headers.Add(new Header("Test", expected));
                };
            });

            var testNumber = 700;
            var complexClass = new ComplexClass(testNumber);

            void OnSpigotOnOpen(object s, EventArrived<ComplexClass> e)
            {
                if (e.EventData.Index == testNumber) Assert.Equal(complexClass.GetHashCode(), e.EventData.GetHashCode());
            }

            Spigot<ComplexClass>.Open += OnSpigotOnOpen;
            Spigot<ComplexClass>.Send(complexClass);
            Spigot<ComplexClass>.Open -= OnSpigotOnOpen;
        }

        public class SimpleClass1
        {
            public SimpleClass1():this(-1)
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
            public ComplexClass():this (-1)
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
    }
}