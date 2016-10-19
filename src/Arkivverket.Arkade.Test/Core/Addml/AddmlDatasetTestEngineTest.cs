using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Core.Addml;
using Arkivverket.Arkade.Core.Addml.Definitions;
using Arkivverket.Arkade.Core.Addml.Processes;
using Arkivverket.Arkade.Test.Util;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

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

            var testSession = new TestSession(new Archive(ArchiveType.Noark3, Uuid.Random(), new DirectoryInfo(@"c:\temp")));
            var addmlDatasetTestEngine = new AddmlDatasetTestEngine(new FlatFileReaderFactory(), new AddmlProcessRunner(addmlDefinition));
            TestSuite testSuite = addmlDatasetTestEngine.RunTests(addmlDefinition, testSession);


            testSuite.Should().NotBeNull();
            testSuite.TestRuns.Should().NotBeNullOrEmpty();

            List<TestRun> analyseFindMinMaxValues = testSuite.TestRuns
                .Where(run => run.TestName == AnalyseFindMinMaxValues.Name)
                .ToList();
            analyseFindMinMaxValues.Count.Should().Be(1);
        }
    }
}