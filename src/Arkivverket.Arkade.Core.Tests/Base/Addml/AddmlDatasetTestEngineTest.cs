using System;
using Arkivverket.Arkade.Core.Tests.Util;
using Xunit.Abstractions;

namespace Arkivverket.Arkade.Core.Tests.Base.Addml
{
    public class AddmlDatasetTestEngineTest : IDisposable
    {
        private readonly IDisposable _logCapture;

        public AddmlDatasetTestEngineTest(ITestOutputHelper outputHelper)
        {
            _logCapture = LoggingHelper.Capture(outputHelper);
        }

        public void Dispose()
        {
            _logCapture.Dispose();
        }

    }
}