using Polly;
using System;

namespace Archetypical.Software.Spigot
{
    /// <summary>
    /// Resilience definitions for the Spigot instance
    /// </summary>
    public class Resilience
    {
        /// <summary>
        /// The resiliency policy
        /// </summary>
        public Resilience()
        {
            Sending = new Retry();
            Receiving = new Retry();
            Lifetimes = new Lifetime();
        }

        /// <summary>
        /// Defines a Retry strategy
        /// </summary>
        public class Retry
        {
            /// <summary>
            /// The number of attempts
            /// </summary>
            public int RetryAttempts { get; set; } = 3;

            /// <summary>
            /// The jitter time
            /// </summary>
            public TimeSpan TimeToWaitBetweenAttempts { get; set; } = TimeSpan.FromSeconds(1);

            /// <summary>
            /// Apply the retry policy
            /// </summary>
            /// <param name="action"></param>
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

        /// <summary>
        /// Specifies the validity of the message
        /// </summary>
        public class Lifetime
        {
            /// <summary>
            /// By default, this is 1 minute
            /// </summary>
            public TimeSpan MessageValidFor { get; set; } = TimeSpan.FromMinutes(1);
        }

        /// <summary>
        /// Retry policy on broadcasting messages
        /// </summary>
        public Retry Sending { get; set; }

        /// <summary>
        /// Retry policy to connect and receive
        /// </summary>
        public Retry Receiving { get; set; }

        /// <summary>
        /// How long messages are valid for
        /// </summary>
        public Lifetime Lifetimes { get; set; }
    }
}