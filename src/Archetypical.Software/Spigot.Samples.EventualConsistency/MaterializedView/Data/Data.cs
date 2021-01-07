using System;

namespace Spigot.Samples.EventualConsistency.MaterializedView.Data
{
    public class Record
    {
        public string Airline { get; set; }

        public string FlightNumber { get; set; }

        public string Gate { get; set; }

        public string Remark { get; set; }

        public string Terminal { get; set; }

        //TIME,AIRLINE,FLIGHT_NUMBER,TRANSACTION,TERMINAL,GATE,REMARK
        public DateTime Time { get; set; }

        public string Transaction { get; set; }

        public static Record FromString(string csv)
        {
            var parts = csv.Split(',');
            return new Record
            {
                Time = DateTime.Parse(parts[0].Replace("\0", "")),
                Airline = parts[1],
                FlightNumber = parts[2],
                Transaction = parts[3],
                Terminal = parts[4],
                Gate = parts[5],
                Remark = parts.Length == 7 ? parts[6] : "",
            };
        }
    }
}