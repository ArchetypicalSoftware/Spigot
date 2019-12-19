using Polly;
using System;

namespace Archetypical.Software.Spigot
{
    /// <summary>
    /// Resilence definitions for the Spigot instance
    /// </summary>
    public class Resilience
    {
        public Resilience()
        {
            Sending = new Retry();
            Receiving = new Retry();
            Lifetimes = new Lifetime();
        }

        public class Retry
        {
            public int RetryAttempts { get; set; } = 3;
            public TimeSpan TimeToWaitBetweenAttempts { get; set; } = TimeSpan.FromSeconds(1);

            public void Execute(Action action)
            {
                Policy.Handle<Exception>().WaitAndRetry(RetryAttempts,
                    i => TimeToWaitBetweenAttempts, (e, t) =>
                        {
                            Console.WriteLine($"Failed with {e}.Reattempting in {t}");
                        })
                    .Execute(action);
            }
        }

        public class Lifetime
        {
            public TimeSpan MessageValidFor { get; set; } = TimeSpan.FromMinutes(1);
        }

        public Retry Sending { get; set; }

        public Retry Receiving { get; set; }

        public Lifetime Lifetimes { get; set; }
    }
}