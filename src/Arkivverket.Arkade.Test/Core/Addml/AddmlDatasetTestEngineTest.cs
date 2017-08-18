using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Core.Addml;
using Arkivverket.Arkade.Core.Addml.Definitions;
using Arkivverket.Arkade.Core.Addml.Processes;
using Arkivverket.Arkade.Logging;
using Arkivverket.Arkade.Test.Util;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;
using Arkivverket.Arkade.Util;

namespace Arkivverket.Arkade.Test.Core.Addml
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