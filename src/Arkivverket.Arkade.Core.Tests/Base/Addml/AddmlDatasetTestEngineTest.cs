using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Base.Addml;
using Arkivverket.Arkade.Core.Base.Addml.Definitions;
using Arkivverket.Arkade.Core.Base.Addml.Processes;
using Arkivverket.Arkade.Core.Logging;
using Arkivverket.Arkade.Core.Tests.Util;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;
using Arkivverket.Arkade.Core.Util;

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