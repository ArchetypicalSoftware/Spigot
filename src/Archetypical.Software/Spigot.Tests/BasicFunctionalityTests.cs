using Archetypical.Software.Spigot;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Threading;
using Xunit;
using Xunit.Abstractions;

namespace Spigot.Tests
{
    public class BasicFunctionalityTests
    {
        private ILoggerFactory factory;

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
                if (e.EventData.Index == testNumber)
                {
                    eventsReceived++;
                }
            }
            void SecondHandler(object s, EventArrived<ComplexClass> e)
            {
                if (e.EventData.Index == testNumber)
                {
                    eventsReceived++;
                }
            }

            Spigot<SimpleClass1>.Open += FirstHandler;
            Spigot<ComplexClass>.Open += SecondHandler;
            Console.WriteLine("sending");
            Spigot<SimpleClass1>.Send(new SimpleClass1(testNumber));
            Spigot<SimpleClass1>.Send(new SimpleClass1(testNumber));
            Spigot<ComplexClass>.Send(new ComplexClass(testNumber));
            WaitFor<SimpleClass1>();
            WaitFor<ComplexClass>();
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
                if (e.EventData.Index == testNumber)
                {
                    eventsReceived++;
                }
            }

            Spigot<SimpleClass1>.Open += OnSpigotOnOpen;
            Spigot<SimpleClass1>.Open += OnSpigotOnOpen;
            Spigot<SimpleClass1>.Send(new SimpleClass1(testNumber));
            WaitFor<SimpleClass1>();
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
                if (e.EventData.Index == testNumber)
                {
                    Interlocked.Increment(ref eventsReceived);
                }
            }
            void SecondHandler(object s, EventArrived<SimpleClass1> e)
            {
                if (e.EventData.Index == testNumber)
                {
                    Interlocked.Increment(ref eventsReceived);
                }
            }
            Spigot<SimpleClass1>.Open += FirstHandler;
            Spigot<SimpleClass1>.Open += SecondHandler;
            Spigot<SimpleClass1>.Send(new SimpleClass1(testNumber));
            WaitFor<SimpleClass1>();
            Assert.Equal(2, eventsReceived);
            Spigot<SimpleClass1>.Open -= FirstHandler;

            Spigot<SimpleClass1>.Send(new SimpleClass1(testNumber));
            Spigot<SimpleClass1>.Open -= SecondHandler;
            WaitFor<SimpleClass1>();
            Assert.Equal(3, eventsReceived);
        }

        private static void WaitFor<T>(TimeSpan? span = null) where T : class, new()
        {
            var iterations = span.GetValueOrDefault(TimeSpan.FromSeconds(2)).TotalMilliseconds / 100;
            while (iterations > 0 && Spigot<T>.HasOutstandingHandles)
            {
                Thread.Sleep(100);
                iterations--;
            }

            Assert.False(Spigot<T>.HasOutstandingHandles, $"Still outstanding events after {span.GetValueOrDefault(TimeSpan.FromSeconds(2)).TotalMilliseconds * 100} ms");
            Assert.Equal(0, Spigot<T>.outstandingThreads);
        }

        [Fact]
        public void Multiple_Handlers_Get_Called_Even_When_One_Errors()
        {
            int eventsReceived = 0;
            var testNumber = 400;

            void OnSpigotOnOpen(object s, EventArrived<SimpleClass1> e) => throw new Exception();
            void OnOpen(object s, EventArrived<SimpleClass1> e)
            {
                if (e.EventData.Index == testNumber)
                {
                    eventsReceived++;
                }
            }
            Spigot<SimpleClass1>.Open += OnSpigotOnOpen;
            Spigot<SimpleClass1>.Open += OnOpen;
            Spigot<SimpleClass1>.Send(new SimpleClass1(testNumber));
            WaitFor<SimpleClass1>();
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
                if (e.EventData.Index == testNumber)
                {
                    eventsClass1Received++;
                }
            }

            void OnOpen(object s, EventArrived<ComplexClass> e)
            {
                if (e.EventData.Index == testNumber)
                {
                    eventsClass2Received++;
                }
            }

            Spigot<ComplexClass>.Open += OnOpen;
            Spigot<SimpleClass1>.Open += OnSpigotOnOpen;

            Spigot<SimpleClass1>.Send(new SimpleClass1(testNumber));
            Spigot<SimpleClass1>.Send(new SimpleClass1(testNumber));
            Spigot<ComplexClass>.Send(new ComplexClass(testNumber));

            WaitFor<SimpleClass1>();
            WaitFor<ComplexClass>();

            Spigot<ComplexClass>.Open -= OnOpen;
            Spigot<SimpleClass1>.Open -= OnSpigotOnOpen;

            Assert.Equal(2, eventsClass1Received);
            Assert.Equal(1, eventsClass2Received);
        }

        [Fact]
        public void Pre_And_Post_Handlers_Work_As_Expected()
        {
            var expected = Guid.NewGuid().ToString();
            Archetypical.Software.Spigot.Spigot.Setup(settings =>
            {
                settings.BeforeSend = env =>
{
    env.Headers.Add(new Header("Test", expected));
};
            });
            var testNumber = 600;

            void OnSpigotOnOpen(object s, EventArrived<SimpleClass1> e)
            {
                if (e.EventData.Index == testNumber)
                {
                    Assert.Equal(expected, e.Context.Headers["Test"]?.Value);
                }
            }

            Spigot<SimpleClass1>.Open += OnSpigotOnOpen;
            Spigot<SimpleClass1>.Send(new SimpleClass1(testNumber));
            Spigot<SimpleClass1>.Open -= OnSpigotOnOpen;
            WaitFor<SimpleClass1>();
        }

        [Fact]
        public void Correct_number_of_iterations()
        {
            var cde = new CountdownEvent(100);
            Spigot<ComplexClass>.Open += (cl, context) => { cde.Signal(); };
            for (var i = 0; i < 100; i++)
            {
                Spigot<ComplexClass>.Send(new ComplexClass());
            }

            cde.Wait(TimeSpan.FromMilliseconds(10 * 100));
            Assert.Equal(0, cde.CurrentCount);
            WaitFor<ComplexClass>();
        }

        [Fact]
        public void Serialization_And_Deserialization_Work_As_Expected()
        {
            var expected = Guid.NewGuid().ToString();
            Archetypical.Software.Spigot.Spigot.Setup(settings =>
            {
                settings.BeforeSend = env =>
                {
                    env.Headers.Add(new Header("Test", expected));
                };
            });

            var testNumber = 700;
            var complexClass = new ComplexClass(testNumber);

            void OnSpigotOnOpen(object s, EventArrived<ComplexClass> e)
            {
                if (e.EventData.Index == testNumber)
                {
                    Assert.Equal(complexClass.GetHashCode(), e.EventData.GetHashCode());
                }
            }

            Spigot<ComplexClass>.Open += OnSpigotOnOpen;
            Spigot<ComplexClass>.Send(complexClass);
            WaitFor<ComplexClass>();
            Spigot<ComplexClass>.Open -= OnSpigotOnOpen;
        }

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
    }
}