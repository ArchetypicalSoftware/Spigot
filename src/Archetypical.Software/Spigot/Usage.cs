using System;

namespace Archetypical.Software.Spigot
{
    public class Usage
    {
        public Usage()
        {
            Archetypical.Software.Spigot.Spigot.Setup(settings => {
                
            });

            Spigot<SomeEvent>.Open += _EventArrived;

            Spigot<SomeEvent>.Send(new SomeEvent { });
        }

        private void _EventArrived(object sender, EventArrived<SomeEvent> e)
        {
            throw new NotImplementedException();
        }
    }
}