using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using Xunit.Abstractions;

namespace Spigot.Tests
{
    internal class XunitLogger : ILogger
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
}