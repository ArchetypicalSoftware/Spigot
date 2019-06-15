using Archetypical.Software.Spigot;
using Spigot.Samples.EventualConsistency.MaterializedView.Data;
using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.IO;
using System.IO.Pipelines;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Spigot.Samples.EventualConsistency.MaterializedView
{
    public class Demonstrator : IDemonstrator
    {
        private readonly ConcurrentDictionary<string, Terminal> Terminals = new ConcurrentDictionary<string, Terminal>();
        private readonly CancellationTokenSource source = new CancellationTokenSource();
        private TextWriter textWriter;

        public void Describe(TextWriter writer)
        {
            textWriter = writer;
            writer.WriteLine(
                @"This demonstrates actual departure and arrival time of all airline flights arriving and departing out of assigned gates and stands at San Francisco International Airport.");
            writer.WriteLine("This will gather events from each gate and compile them into a materialized view for people checking the status of gates and availability ");
        }

        public void Go(TextWriter writer)
        {
            Spigot<Record>.Open += (eh, a) => writer.WriteLine($"{a.EventData.Time.ToShortDateString()} - Terminal {a.EventData.Terminal} Gate {a.EventData.Gate} : {a.EventData.Airline} flight number {a.EventData.FlightNumber} {(a.EventData.Transaction == "ARR" ? "Arrived" : "Departed")}");

            using (var str =
                new WebClient().OpenRead("https://data.sfgov.org/api/views/chfu-j7tc/rows.csv?accessType=DOWNLOAD"))
            {
                ProcessStreamAsync(str).GetAwaiter().GetResult();
            }

            source.CancelAfter(TimeSpan.FromSeconds(10));
            writer.WriteLine("Cancelling all operations in 10 seconds...");
        }

        private async Task FillPipeAsync(Stream stream, PipeWriter writer)

        {
            const int minimumBufferSize = 2 * 1024;
            while (true)
            {
                // Allocate at least 512 bytes from the PipeWriter
                var memory = writer.GetMemory(minimumBufferSize);
                try
                {
                    var bytesRead = await stream.ReadAsync(memory, source.Token);
                    // Tell the PipeWriter how much was read from the Socket
                    writer.Advance(bytesRead);
                    if (bytesRead == 0)
                    {
                        break;
                    }
                }
                catch (Exception ex)
                {
                    textWriter.WriteLine("ERROR::: " + ex.ToString());
                    continue;
                }

                // Make the data available to the PipeReader
                var result = await writer.FlushAsync(source.Token);
                if (result.IsCompleted || result.IsCanceled)
                {
                    break;
                }
            }

            // Tell the PipeReader that there's no more data coming
            writer.Complete();
        }

        private async Task ProcessStreamAsync(Stream str)
        {
            var pipe = new Pipe();
            var writing = FillPipeAsync(str, pipe.Writer);
            var reading = ReadPipeAsync(pipe.Reader);
            await Task.WhenAll(reading, writing);
        }

        private async Task ReadPipeAsync(PipeReader reader)
        {
            while (true)
            {
                var result = await reader.ReadAsync();
                var buffer = result.Buffer;
                SequencePosition? position = null;
                do
                {
                    // Look for a EOL in the buffer
                    position = buffer.PositionOf((byte)'\n');
                    if (position != null)
                    {
                        // Process the line
                        var portion = buffer.Slice(0, position.Value);
                        var line = System.Text.Encoding.Default.GetString(portion.ToArray());
                        var isHeader = line[0] == 'T';
                        if (!isHeader)
                        {
                            var record = Record.FromString(line);
                            var terminal = Terminals.GetOrAdd(record.Terminal, new Terminal(record.Terminal));
                            var gate = terminal.Gates.GetOrAdd(record.Gate, new Gate(record.Gate, source));
                            gate.AddEvent(record);
                        }

                        // Skip the line + the \n character (basically position)
                        buffer = buffer.Slice(buffer.GetPosition(1, position.Value));
                    }
                } while (position != null);

                // Tell the PipeReader how much of the buffer we have consumed
                try
                {
                    reader.AdvanceTo(buffer.Start, buffer.End);
                }
                catch (InvalidOperationException)
                {
                }
                // Stop reading if there's no more data coming
                if (result.IsCompleted)
                {
                    break;
                }
            }

            // Mark the PipeReader as complete
            reader.Complete();
        }
    }
}