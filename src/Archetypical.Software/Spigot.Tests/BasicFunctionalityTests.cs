using Archetypical.Software.Spigot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Spigot.Tests
{
    public class BasicFunctionalityTests
    {
        [Fact]
        public void WorksOutOfTheBox()
        {
            int eventsRecieved=0;
            Spigot<SimpleClass1>.Open += (s, e) => { eventsRecieved++; };
            Spigot<SimpleClass2>.Open += (s, e) => { eventsRecieved++; };

            Spigot<SimpleClass1>.Send(new SimpleClass1());
            Spigot<SimpleClass1>.Send(new SimpleClass1());
            Spigot<SimpleClass2>.Send(new SimpleClass2());

            Assert.Equal(3, eventsRecieved);
        }

        [Fact]
        public void Routes_Correctly()
        {
            int eventsClass1Recieved = 0;
            int eventsClass2Recieved = 0;

            Spigot<SimpleClass1>.Open += (s, e) => { eventsClass1Recieved++; };
            Spigot<SimpleClass2>.Open += (s, e) => { eventsClass2Recieved++; };

            Spigot<SimpleClass1>.Send(new SimpleClass1());
            Spigot<SimpleClass1>.Send(new SimpleClass1());
            Spigot<SimpleClass2>.Send(new SimpleClass2());

            Assert.Equal(2, eventsClass1Recieved);
            Assert.Equal(1, eventsClass2Recieved);
        }


        public class SimpleClass1
        {

        }

        public class SimpleClass2
        {

        }
    }
}
