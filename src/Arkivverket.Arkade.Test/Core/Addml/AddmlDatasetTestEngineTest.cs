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

        [Fact]
        public void ShouldReturnTestSuiteFromTests()
        {
            AddmlInfo addml = AddmlUtil.ReadFromBaseDirectory("..\\..\\TestData\\noark3\\noark_3_arkivuttrekk_med_prosesser.xml");
            AddmlDefinition addmlDefinition = new AddmlDefinitionParser(addml).GetAddmlDefinition();

            var testSession = new TestSession(new Archive(ArchiveType.Noark3, Uuid.Random(), ArkadeConstants.GetArkadeWorkDirectory()));
            testSession.AddmlDefinition = addmlDefinition;

            var addmlDatasetTestEngine = new AddmlDatasetTestEngine(new FlatFileReaderFactory(), new AddmlProcessRunner(), new StatusEventHandler());
            TestSuite testSuite = addmlDatasetTestEngine.RunTestsOnArchive(testSession);


            testSuite.Should().NotBeNull();
            testSuite.TestRuns.Should().NotBeNullOrEmpty();

            List<TestRun> analyseFindMinMaxValues = testSuite.TestRuns
                .Where(run => run.TestName == AnalyseFindMinMaxValues.Name)
                .ToList();
            analyseFindMinMaxValues.Count.Should().Be(1);
        }
    }
}