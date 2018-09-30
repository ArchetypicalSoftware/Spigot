using System;
using Archetypical.Software.Spigot;
using Xunit;

namespace Spigot.Tests
{
    public class BasicFunctionalityTests
    {
        [Fact]
        public void WorksOutOfTheBox()
        {
            int eventsReceived = 0;
            Spigot<SimpleClass1>.Open += (s, e) => { eventsReceived++; };
            Spigot<SimpleClass2>.Open += (s, e) => { eventsReceived++; };

            Spigot<SimpleClass1>.Send(new SimpleClass1());
            Spigot<SimpleClass1>.Send(new SimpleClass1());
            Spigot<SimpleClass2>.Send(new SimpleClass2());

            Assert.Equal(3, eventsReceived);
        }

        [Fact]
        public void Multiple_Handlers_Get_Called()
        {
            int eventsReceived = 0;
            Spigot<SimpleClass1>.Open += (s, e) =>
            {
                eventsReceived++;
            };
            Spigot<SimpleClass1>.Open += (s, e) =>
            {
                eventsReceived++;
            };
            Spigot<SimpleClass1>.Send(new SimpleClass1());

            Assert.Equal(2, eventsReceived);
        }

        [Fact]
        public void Handlers_Can_Be_Added_And_Removed()
        {
            int eventsReceived = 0;

            void FirstHandler(object s, EventArrived<SimpleClass1> e)
            {
                eventsReceived++;
            }
            void SecondHandler(object s, EventArrived<SimpleClass1> e)
            {
                eventsReceived++;
            }
            Spigot<SimpleClass1>.Open += FirstHandler;
            Spigot<SimpleClass1>.Open += SecondHandler;
            Spigot<SimpleClass1>.Send(new SimpleClass1());
            Spigot<SimpleClass1>.Open -= FirstHandler;

            Spigot<SimpleClass1>.Send(new SimpleClass1());
            Assert.Equal(3, eventsReceived);
        }

        [Fact]
        public void Multiple_Handlers_Get_Called_Even_When_One_Errors()
        {
            int eventsReceived = 0;
            Spigot<SimpleClass1>.Open += (s, e) => throw new Exception();
            Spigot<SimpleClass1>.Open += (s, e) => eventsReceived++;
            Spigot<SimpleClass1>.Send(new SimpleClass1());

            Assert.Equal(1, eventsReceived);
        }

        [Fact]
        public void Routes_Correctly()
        {
            int eventsClass1Received = 0;
            int eventsClass2Received = 0;

            Spigot<SimpleClass1>.Open += (s, e) => { eventsClass1Received++; };
            Spigot<SimpleClass2>.Open += (s, e) => { eventsClass2Received++; };

            Spigot<SimpleClass1>.Send(new SimpleClass1());
            Spigot<SimpleClass1>.Send(new SimpleClass1());
            Spigot<SimpleClass2>.Send(new SimpleClass2());

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
                }; });

            Spigot<SimpleClass1>.Open += (s, e) => { Assert.Equal(expected, e.Context.Headers["Test"]?.Value); };
            Spigot<SimpleClass1>.Send(new SimpleClass1());
        }

        public class SimpleClass1
        {
        }

        public class SimpleClass2
        {
        }
    }
}