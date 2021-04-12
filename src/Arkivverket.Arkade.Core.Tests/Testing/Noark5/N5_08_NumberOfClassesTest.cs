using System.Linq;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Testing.Noark5;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Core.Tests.Testing.Noark5
{
    public class N5_08_NumberOfClassesTest : LanguageDependentTest
    {
        [Fact]
        public void NumberOfClassesIsTwoInEachArchivePart()
        {
            XmlElementHelper helper = new XmlElementHelper().Add("arkiv",
                new XmlElementHelper()
                    // Arkivdel 1
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("systemID", "someSystemId_1")
                        .Add("tittel", "arkivdelTittel1")
                        // Primært klassifikasjonssystem (inneholder registrering eller mappe)
                        .Add("klassifikasjonssystem", new XmlElementHelper()
                            .Add("systemID", "klassSys_1")
                            .Add("registrering", new XmlElementHelper()
                                .Add("klasse", string.Empty)
                                .Add("klasse", new XmlElementHelper()
                                    .Add("klasse", new XmlElementHelper()))))
                        // Sekundært klassifikasjonssystem
                        .Add("klassifikasjonssystem", new XmlElementHelper()
                            .Add("systemID", "klassSys_2")
                            .Add("klasse", string.Empty)
                            .Add("klasse", new XmlElementHelper()
                                .Add("klasse", new XmlElementHelper()))))
                    // Arkivdel 2
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("systemID", "someSystemId_2")
                        .Add("tittel", "arkivdelTittel2")
                        // Primært klassifikasjonssystem (inneholder registrering eller mappe)
                        .Add("klassifikasjonssystem", new XmlElementHelper()
                            .Add("systemID", "klassSys_1")
                            .Add("mappe", new XmlElementHelper()
                                .Add("klasse", string.Empty)
                                .Add("klasse", new XmlElementHelper()
                                    .Add("klasse", new XmlElementHelper()))))
                        // Sekundært klassifikasjonssystem
                        .Add("klassifikasjonssystem", new XmlElementHelper()
                            .Add("systemID", "klassSys_2")
                            .Add("klasse", string.Empty)
                            .Add("klasse", new XmlElementHelper()
                                .Add("klasse", new XmlElementHelper())))));

            TestRun testRun = helper.RunEventsOnTest(new N5_08_NumberOfClasses());

            testRun.Results.First().Message.Should().Be("Totalt: 12");
            testRun.Results.Should().Contain(r =>
                r.Message.Equals("Arkivdel (systemID, tittel): someSystemId_1, arkivdelTittel1 Primært klassifikasjonssystem (systemID): klassSys_1 - Totalt antall klasser: 3"));
            testRun.Results.Should().Contain(r =>
                r.Message.Equals("Arkivdel (systemID, tittel): someSystemId_1, arkivdelTittel1 Primært klassifikasjonssystem (systemID): klassSys_1 - Klasser på nivå 1: 2"));
            testRun.Results.Should().Contain(r =>
                r.Message.Equals("Arkivdel (systemID, tittel): someSystemId_1, arkivdelTittel1 Primært klassifikasjonssystem (systemID): klassSys_1 - Klasser på nivå 2: 1"));

            testRun.Results.Should().Contain(r =>
                r.Message.Equals("Arkivdel (systemID, tittel): someSystemId_1, arkivdelTittel1 Sekundært klassifikasjonssystem (systemID): klassSys_2 - Totalt antall klasser: 3"));
            testRun.Results.Should().Contain(r =>
                r.Message.Equals("Arkivdel (systemID, tittel): someSystemId_1, arkivdelTittel1 Sekundært klassifikasjonssystem (systemID): klassSys_2 - Klasser på nivå 1: 2"));
            testRun.Results.Should().Contain(r =>
                r.Message.Equals("Arkivdel (systemID, tittel): someSystemId_1, arkivdelTittel1 Sekundært klassifikasjonssystem (systemID): klassSys_2 - Klasser på nivå 2: 1"));

            testRun.Results.Should().Contain(r =>
                r.Message.Equals("Arkivdel (systemID, tittel): someSystemId_2, arkivdelTittel2 Primært klassifikasjonssystem (systemID): klassSys_1 - Totalt antall klasser: 3"));
            testRun.Results.Should().Contain(r =>
                r.Message.Equals("Arkivdel (systemID, tittel): someSystemId_2, arkivdelTittel2 Primært klassifikasjonssystem (systemID): klassSys_1 - Klasser på nivå 1: 2"));
            testRun.Results.Should().Contain(r =>
                r.Message.Equals("Arkivdel (systemID, tittel): someSystemId_2, arkivdelTittel2 Primært klassifikasjonssystem (systemID): klassSys_1 - Klasser på nivå 2: 1"));

            testRun.Results.Should().Contain(r =>
                r.Message.Equals("Arkivdel (systemID, tittel): someSystemId_2, arkivdelTittel2 Sekundært klassifikasjonssystem (systemID): klassSys_2 - Totalt antall klasser: 3"));
            testRun.Results.Should().Contain(r =>
                r.Message.Equals("Arkivdel (systemID, tittel): someSystemId_2, arkivdelTittel2 Sekundært klassifikasjonssystem (systemID): klassSys_2 - Klasser på nivå 1: 2"));
            testRun.Results.Should().Contain(r =>
                r.Message.Equals("Arkivdel (systemID, tittel): someSystemId_2, arkivdelTittel2 Sekundært klassifikasjonssystem (systemID): klassSys_2 - Klasser på nivå 2: 1"));
        }

        [Fact]
        public void ArchiveIsWithoutClassificationSystem()
        {
            XmlElementHelper helper = new XmlElementHelper()
                .Add("arkiv", new XmlElementHelper()
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("systemID", "someSystemId_1")
                        .Add("registrering", string.Empty))
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("systemID", "someSystemId_2")
                        .Add("mappe", string.Empty)));

            TestRun testRun = helper.RunEventsOnTest(new N5_08_NumberOfClasses());

            testRun.Results.First().Message.Should().Be("Totalt: 0");
        }
    }
}
