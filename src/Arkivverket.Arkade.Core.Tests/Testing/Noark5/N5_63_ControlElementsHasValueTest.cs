using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Testing.Noark5;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Core.Tests.Testing.Noark5
{
    public class N5_63_ControlElementsHasContentTest : LanguageDependentTest
    {
        [Fact]
        public void ElementsWithoutContentAreReported()
        {
            XmlElementHelper helper = new XmlElementHelper()
                .Add("arkiv", new XmlElementHelper()
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("systemID", "someSystemId_1")
                        .Add("klassifikasjonssystem", new XmlElementHelper()
                            .Add("klasse", new XmlElementHelper()
                                .Add("mappe", new XmlElementHelper()
                                    .Add("registrering", new XmlElementHelper()
                                        .Add("dokumentbeskrivelse", new XmlElementHelper()
                                            .Add("systemID", "someSystemId_2")
                                            .Add("tittel", string.Empty))))))));

            TestRun testRun = helper.RunEventsOnTest(new N5_63_ControlElementsHasContent());

            testRun.Results.Count.Should().Be(1);
            testRun.Results.Should().Contain(r => r.Location.ToString().Equals("Etter systemID someSystemId_2"));
            testRun.Results.Should().Contain(r => r.Message.Equals("Elementet tittel mangler innhold"));
        }

        [Fact]
        public void ElementsWithContentAreNotReported()
        {
            XmlElementHelper helper = new XmlElementHelper()
                .Add("arkiv", new XmlElementHelper()
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("systemID", "someSystemId_1")
                        .Add("klassifikasjonssystem", new XmlElementHelper()
                            .Add("klasse", new XmlElementHelper()
                                .Add("mappe", new XmlElementHelper()
                                    .Add("registrering", new XmlElementHelper()
                                        .Add("dokumentbeskrivelse", new XmlElementHelper()
                                            .Add("systemID", "someSystemId_2")
                                            .Add("tittel", "Some title"))))))));

            TestRun testRun = helper.RunEventsOnTest(new N5_63_ControlElementsHasContent());

            testRun.Results.Count.Should().Be(0);
        }
    }
}
