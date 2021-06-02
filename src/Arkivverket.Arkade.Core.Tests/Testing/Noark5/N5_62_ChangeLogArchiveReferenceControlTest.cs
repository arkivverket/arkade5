using System.Linq;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Testing.Noark5;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Core.Tests.Testing.Noark5
{
    public class N5_62_ChangeLogArchiveReferenceControlTest
    {
        [Fact]
        public void ReferencesIsValid()
        {
            XmlElementHelper xmlElementHelper =
                new XmlElementHelper().Add("arkiv",
                    new XmlElementHelper().Add("arkivdel",
                        new XmlElementHelper().Add("klassifikasjonssystem",
                            new XmlElementHelper().Add("klasse",
                                new XmlElementHelper()
                                    .Add("mappe",
                                        new XmlElementHelper().Add("systemID", "734b493f-c64e-4fc5-a988-56be11e2ee10"))
                                    .Add("mappe",
                                        new XmlElementHelper().Add("systemID", "214e27a2-5e7f-484b-b2c2-dea4e50524a3"))))));

            const string testdataDirectory = "TestData\\Noark5\\LogsControl";
            // Changelog references archive units (systemID):
            // 734b493f-c64e-4fc5-a988-56be11e2ee10
            // 214e27a2-5e7f-484b-b2c2-dea4e50524a3

            Archive testArchive = TestUtil.CreateArchiveExtraction(testdataDirectory);
            TestRun testRun = xmlElementHelper.RunEventsOnTest(new N5_62_ChangeLogArchiveReferenceControl(testArchive));

            testRun.TestResults.GetNumberOfResults().Should().Be(0);
        }

        [Fact]
        public void SomeReferencesAreInvalid()
        {
            XmlElementHelper xmlElementHelper =
                new XmlElementHelper().Add("arkiv",
                    new XmlElementHelper().Add("arkivdel",
                        new XmlElementHelper().Add("klassifikasjonssystem",
                            new XmlElementHelper().Add("klasse",
                                new XmlElementHelper().Add("mappe",
                                    new XmlElementHelper().Add("systemID", "734b493f-c64e-4fc5-a988-56be11e2ee10"))))));

            const string testdataDirectory = "TestData\\Noark5\\LogsControl";
            // Changelog references archive units (systemID):
            // 734b493f-c64e-4fc5-a988-56be11e2ee10
            // 214e27a2-5e7f-484b-b2c2-dea4e50524a3

            Archive testArchive = TestUtil.CreateArchiveExtraction(testdataDirectory);
            TestRun testRun = xmlElementHelper.RunEventsOnTest(new N5_62_ChangeLogArchiveReferenceControl(testArchive));

            testRun.TestResults.TestsResults.First().Message.Should().Be(
                "Referanse til arkivenhet er ikke gyldig: (systemID) 214e27a2-5e7f-484b-b2c2-dea4e50524a3"
            );
        }
    }
}
