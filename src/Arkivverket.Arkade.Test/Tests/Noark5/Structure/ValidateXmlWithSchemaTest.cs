using System;
using System.IO;
using System.Text;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Tests;
using Arkivverket.Arkade.Tests.Noark5.Structure;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace Arkivverket.Arkade.Test.Tests.Noark5.Structure
{
    public class ValidateXmlWithSchemaTest : IDisposable
    {
        public ValidateXmlWithSchemaTest(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
            _archiveContent = ValidContentStream();
            _archiveStructureContent = ValidStructureStream();
        }

        private Stream ValidStructureStream()
        {
            var xml = @"<?xml version=""1.0"" encoding=""utf-8""?>" +
                      @"<addml xmlns=""http://www.arkivverket.no/standarder/addml"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">" +
                      @"<dataset><description>test description</description><reference></reference></dataset></addml>";
            return GenerateStreamFromString(xml);
        }

        private Stream ValidContentStream()
        {
            var xml = @"<?xml version=""1.0"" encoding=""UTF-8""?>" +
                      @"<arkiv xmlns=""http://www.arkivverket.no/standarder/noark5/arkivstruktur"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">" +
                      @"  <systemID/>" +
                      @"  <tittel/>" +
                      @"  <opprettetDato>2001-12-17T09:30:47Z</opprettetDato>" +
                      @"  <opprettetAv/>" +
                      @"  <avsluttetDato>2001-12-17T09:30:47Z</avsluttetDato>" +
                      @"  <avsluttetAv/>" +
                      @"  <arkivskaper>" +
                      @"    <arkivskaperID/>" +
                      @"    <arkivskaperNavn/>" +
                      @"  </arkivskaper>" +
                      @"  <arkivdel>" +
                      @"    <systemID/>" +
                      @"    <tittel/>" +
                      @"    <arkivdelstatus>Aktiv periode</arkivdelstatus>" +
                      @"    <opprettetDato>2001-12-17T09:30:47Z</opprettetDato>" +
                      @"    <opprettetAv/>" +
                      @"    <avsluttetDato>2001-12-17T09:30:47Z</avsluttetDato>" +
                      @"    <avsluttetAv/>" +
                      @"  </arkivdel>" +
                      @"</arkiv>";
            return GenerateStreamFromString(xml);
        }

        public void Dispose()
        {
            _archiveStructureContent?.Dispose();
            _archiveContent?.Dispose();
        }

        private readonly ITestOutputHelper _outputHelper;

        private Stream _archiveStructureContent;
        private Stream _archiveContent;

        private TestRun RunTest()
        {
            Archive archive = new Core.ArchiveBuilder().WithWorkingDirectoryRoot("c:\\temp").Build();
            var validateXmlWithSchema = new ValidateXmlWithSchema(new ArchiveContentMockReader(_archiveContent, _archiveStructureContent));
            validateXmlWithSchema.Test(archive);
            return validateXmlWithSchema.GetTestRun();
        }

        private MemoryStream GenerateStreamFromString(string value)
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(value ?? ""));
        }

        [Fact(Skip = "Await archive files handling refactoring")]
        public void ShouldReturnErrorsWhenAddmlXmlIsInvalidAccordingToSchema()
        {
            var xml =
                @"<?xml version=""1.0"" encoding=""utf-8""?><addml xmlns=""http://www.arkivverket.no/standarder/addml""><hello></hello></addml>";
            _archiveStructureContent = GenerateStreamFromString(xml);

            RunTest().Results.Should().Contain(r => r.IsError() && r.Message.Contains("hello"));
        }

        [Fact(Skip = "Await archive files handling refactoring")]
        public void ShouldReturnErrorsWhenArkivstrukturXmlIsInvalidAccordingToSchema()
        {
            var xml =
                @"<?xml version=""1.0"" encoding=""utf-8""?><arkiv xmlns=""http://www.arkivverket.no/standarder/noark5/arkivstruktur""><hello></hello></arkiv>";
            _archiveContent = GenerateStreamFromString(xml);

            RunTest().Results.Should().Contain(r => r.IsError() && r.Message.Contains("hello"));
        }

        [Fact(Skip = "Interferred by test data from other tests ...")]
        public void ShouldReturnSuccessWhenBothArkivuttrekkAndArkivstrukturIsValidXml()
        {
            var testResults = RunTest();
            testResults.IsSuccess().Should().BeTrue();

            foreach (var testResult in testResults.Results)
            {
                _outputHelper.WriteLine(
                    (!string.IsNullOrEmpty(testResult.Location.ToString()) ? testResult.Location + ": " : string.Empty)
                    + testResult.Message
                );
            }
        }
    }
}