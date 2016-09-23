using System;
using System.IO;
using System.Text;
using Arkivverket.Arkade.Core;
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
        }

        public void Dispose()
        {
            _archiveStructureContent?.Dispose();
        }

        private readonly ITestOutputHelper _outputHelper;

        private Stream _archiveStructureContent;

        private TestRun RunTest()
        {
            return new ValidateXmlWithSchema(new ArchiveContentMockReader(_archiveStructureContent)).RunTest(new Archive("", ""));
        }

        private MemoryStream GenerateStreamFromString(string value)
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(value ?? ""));
        }

        [Fact]
        public void ShouldReturnErrorsWhenXmlIsInvalidAccordingToSchema()
        {
            var xml =
                @"<?xml version=""1.0"" encoding=""utf-8""?><addml xmlns=""http://www.arkivverket.no/standarder/addml""><hello></hello></addml>";
            _archiveStructureContent = GenerateStreamFromString(xml);
            var testResults = RunTest();

            _outputHelper.WriteLine(testResults.Results[0].Message);

            testResults.IsSuccess().Should().BeFalse();
        }

        [Fact]
        public void ShouldReturnSuccessWhenValidXml()
        {
            var xml = @"<?xml version=""1.0"" encoding=""utf-8""?>" +
                      @"<addml xmlns=""http://www.arkivverket.no/standarder/addml"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">" +
                      @"<dataset><description>test description</description><reference></reference></dataset></addml>";

            _archiveStructureContent = GenerateStreamFromString(xml);
            var testResults = RunTest();

            _outputHelper.WriteLine(testResults.Results[0].Message);

            testResults.IsSuccess().Should().BeTrue();
        }
    }
}