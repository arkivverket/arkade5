using System;
using System.IO;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Core.Addml;
using Arkivverket.Arkade.Core.Addml.Definitions;
using Arkivverket.Arkade.ExternalModels.Addml;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Test.Core.Addml
{
    public class AddmlDatasetTestEngineTest
    {
        //[Fact(Skip = "Could not find file 'SAK.DAT'")]
        [Fact]
        public void ShouldReturnTestSuiteFromTests()
        {
            AddmlInfo addml = AddmlUtil.ReadFromBaseDirectory("..\\..\\TestData\\noark3\\noark_3_arkivuttrekk_med_prosesser.xml");
            AddmlDefinition addmlDefinition = new AddmlDefinitionParser(addml).GetAddmlDefinition();

            var testSession = new TestSession(new Archive(ArchiveType.Noark3, Uuid.Random(), new DirectoryInfo(@"c:\temp")));
            var addmlDatasetTestEngine = new AddmlDatasetTestEngine(new FlatFileReaderFactory(), new AddmlProcessRunner());
            TestSuite testSuite = addmlDatasetTestEngine.RunTests(addmlDefinition, testSession);
            testSuite.Should().NotBeNull();
        }
    }
}