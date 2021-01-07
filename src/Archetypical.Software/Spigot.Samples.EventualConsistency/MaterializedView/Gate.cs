using Archetypical.Software.Spigot;
using Spigot.Samples.EventualConsistency.MaterializedView.Data;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Spigot.Samples.EventualConsistency.MaterializedView
{
    public class Gate
    {
        private readonly MessageSender<Record> _sender;
        public string GateNumber { get; internal set; }
        public CancellationTokenSource Source { get; }
        private Random r = new Random(DateTime.Now.Millisecond);

        public Gate(string number, MessageSender<Record> sender, CancellationTokenSource source)
        {
            _sender = sender;
            GateNumber = number;
            Source = source;
            RunSimulatingDelay().Start();
        }

        private async Task RunSimulatingDelay()
        {
            while (!Source.IsCancellationRequested)
            {
                await Task.Delay(r.Next(0, 1000));
                while (_gateTime.TryDequeue(out Record record))
                {
                    _sender.Send(record);
                }
            }
        }

        private readonly ConcurrentQueue<Record> _gateTime = new ConcurrentQueue<Record>();

        public void AddEvent(Record record)
        {
            _gateTime.Enqueue(record);
        }
    }
}