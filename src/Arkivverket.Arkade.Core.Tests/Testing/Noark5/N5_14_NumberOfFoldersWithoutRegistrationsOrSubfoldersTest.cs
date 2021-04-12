using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Testing.Noark5;
using FluentAssertions;
using System.Linq;
using Xunit;

namespace Arkivverket.Arkade.Core.Tests.Testing.Noark5
{
    public class N5_14_NumberOfFoldersWithoutRegistrationsOrSubfoldersTest : LanguageDependentTest
    {
        [Fact]
        public void ResultIsNoFoldersWithoutRegistrationsOrSubfolders()
        {
            XmlElementHelper helper = new XmlElementHelper().Add("arkiv",
                new XmlElementHelper()
                    .Add("arkivdel",
                        new XmlElementHelper().Add("klassifikasjonssystem",
                            new XmlElementHelper().Add("klasse",
                                new XmlElementHelper()
                                    .Add("mappe",
                                        new XmlElementHelper().Add("registrering",
                                            new XmlElementHelper().Add("someelement", "some value")))
                                    .Add("mappe", // Folder has no registration but has a subfolder
                                        new XmlElementHelper()
                                            .Add("mappe",
                                                new XmlElementHelper().Add("registrering",
                                                    new XmlElementHelper().Add("someelement", "some value"))))))));

            TestRun testRun = helper.RunEventsOnTest(new N5_14_NumberOfFoldersWithoutRegistrationsOrSubfolders());

            testRun.Results.First().Message.Should().Be("Totalt: 0");
        }

        [Fact]
        public void ResultIsSomeFoldersWithoutRegistrationsOrSubfolders()
        {
            XmlElementHelper helper = new XmlElementHelper().Add("arkiv",
                new XmlElementHelper()
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("klassifikasjonssystem", new XmlElementHelper()
                            .Add("klasse", new XmlElementHelper()
                                .Add("mappe", new XmlElementHelper()
                                    .Add("registrering", new XmlElementHelper()
                                        .Add("someelement", "some value")))
                                .Add("mappe", new XmlElementHelper() // Folder has no registration but has a subfolder
                                    .Add("mappe",
                                        new XmlElementHelper())))))); // Folder has neither registration or subfolder

            TestRun testRun = helper.RunEventsOnTest(new N5_14_NumberOfFoldersWithoutRegistrationsOrSubfolders());

            testRun.Results.First().Message.Should().Be("Totalt: 1");
        }

        [Fact]
        public void ResultIsTwoFoldersWithoutRegistrationsOrSubfolders()
        {
            XmlElementHelper helper = new XmlElementHelper().Add("arkiv",
                new XmlElementHelper()
                    .Add("arkivdel",
                        new XmlElementHelper().Add("klassifikasjonssystem",
                            new XmlElementHelper().Add("klasse",
                                new XmlElementHelper()
                                    .Add("mappe", // Folder has neither registration or subfolder
                                        new XmlElementHelper().Add("someelement", "some value"))
                                    .Add("mappe", // Folder has neither registration or subfolder
                                        new XmlElementHelper().Add("someelement", "some value"))))));

            TestRun testRun = helper.RunEventsOnTest(new N5_14_NumberOfFoldersWithoutRegistrationsOrSubfolders());

            testRun.Results.First().Message.Should().Be("Totalt: 2");
        }

        [Fact]
        public void ResultIsTwoFoldersWithoutRegistrationsOrSubfoldersInOneOfTwoArchiveParts()
        {
            XmlElementHelper helper = new XmlElementHelper().Add("arkiv",
                new XmlElementHelper()
                    .Add("arkivdel", new XmlElementHelper()
                        .Add("systemID", "someSystemId_1")
                        .Add("tittel", "someTitle_1")
                        .Add("klassifikasjonssystem", new XmlElementHelper()
                            .Add("klasse", new XmlElementHelper()
                                .Add("mappe", // Folder has neither registration or subfolder
                                    new XmlElementHelper())
                                .Add("mappe", // Folder has neither registration or subfolder
                                    new XmlElementHelper()))))
                   .Add("arkivdel", new XmlElementHelper()
                        .Add("systemID", "someSystemId_2")
                        .Add("tittel", "someTitle_2")
                        .Add("klassifikasjonssystem", new XmlElementHelper()
                            .Add("klasse", new XmlElementHelper()
                                .Add("mappe", new XmlElementHelper()
                                    .Add("registrering", new XmlElementHelper()))
                                .Add("mappe", new XmlElementHelper() // Folder has no registration but has a subfolder
                                    .Add("mappe", new XmlElementHelper()
                                        .Add("registrering", new XmlElementHelper())))))));


            TestRun testRun = helper.RunEventsOnTest(new N5_14_NumberOfFoldersWithoutRegistrationsOrSubfolders());

            testRun.Results.Should().Contain(r => r.Message.Equals("Totalt: 2"));
            testRun.Results.Should().Contain(r => r.Message.Equals("Arkivdel (systemID - tittel) someSystemId_1 - someTitle_1: 2"));
        }
    }
}
