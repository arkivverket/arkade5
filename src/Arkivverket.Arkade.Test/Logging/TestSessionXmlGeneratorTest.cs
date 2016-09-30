using Arkivverket.Arkade.Logging;
using System;
using Xunit;
using FluentAssertions;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Test.Core;

namespace Arkivverket.Arkade.Test.Logging
{
    public class TestSessionXmlGeneratorTest
    {


        [Fact]
        public void XmlShouldContainBasicData()
        {
            TestSession testSession = new TestSessionBuilder()
                .Build();

            string xml = TestSessionXmlGenerator.GenerateXml(testSession);

            TestSessionLogXmlValidator.Validate(xml);
            xml.Should()
                // TODO jostein: Should we have xsi:type in XML?
                .Contain("<timestamp xsi:type=\"xsd:dateTime\">")
                .And
                .Contain("<archiveUuid>" + testSession.Archive.Uuid.GetValue() + "</archiveUuid>")
                .And
                .Contain("<archiveType>" + testSession.Archive.ArchiveType + "</archiveType>")
                .And
                .Contain("<arkadeVersion>unknown</arkadeVersion>")
                ;
        }

        [Fact]
        public void XmlShouldContainLogEntries()
        {
            TestSession testSession = new TestSessionBuilder()
                .WithLogEntry("Log line 1")
                .WithLogEntry("Log line 2")
                .WithLogEntry("Log line 3")
                .WithLogEntry("Log line 4")
                .Build();

            string xml = TestSessionXmlGenerator.GenerateXml(testSession);

            TestSessionLogXmlValidator.Validate(xml);

            xml.Should()
                .Contain("<message>Log line 1</message>")
                .And
                .Contain("<message>Log line 2</message>")
                .And
                .Contain("<message>Log line 3</message>")
                .And
                .Contain("<message>Log line 4</message>");
        }

        [Fact(Skip = "Work in progress!")]
        public void XmlShouldContainTestResults()
        {
            TestSession testSession = new TestSessionBuilder()
                .Build();

            string xml = TestSessionXmlGenerator.GenerateXml(testSession);

            TestSessionLogXmlValidator.Validate(xml);
            xml.Should()
                .Contain("<testName>testName</testName>")
                .And
                .Contain("<testCategory>testCategory</testCategory>")
                .And
                .Contain("<durationMillis>1234</durationMillis>")
                .And
                .Contain("<status>SUCCESS</status>")
                .And
                .Contain("<message>No message</message>");
        }

    }
}
