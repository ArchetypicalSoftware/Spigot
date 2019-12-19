using Microsoft.Extensions.Logging;
using System;
using Xunit.Abstractions;

namespace Spigot.Tests
{
    internal class XunitLoggerProvider : ILoggerProvider
    {
        private ITestOutputHelper _outputHelper;

        public XunitLoggerProvider(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new XunitLogger(_outputHelper);
        }
    }
}