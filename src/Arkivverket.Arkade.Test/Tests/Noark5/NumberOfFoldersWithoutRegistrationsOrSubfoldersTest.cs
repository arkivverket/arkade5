using System.Linq;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Tests.Noark5;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Test.Tests.Noark5
{
    public class NumberOfFoldersWithoutRegistrationsOrSubfoldersTest
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

            TestRun testRun = helper.RunEventsOnTest(new NumberOfFoldersWithoutRegistrationsOrSubfolders());

            testRun.Results.First().Message.Should().Be("0");
        }

        [Fact]
        public void ResultIsSomeFoldersWithoutRegistrationsOrSubfolders()
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
                                            .Add("mappe", // Folder has neither registration or subfolder
                                                new XmlElementHelper().Add("someelement", "some value")))))));

            TestRun testRun = helper.RunEventsOnTest(new NumberOfFoldersWithoutRegistrationsOrSubfolders());

            testRun.Results.First().Message.Should().Be("1");
        }

        [Fact]
        public void ResultIsOnlyFoldersWithoutRegistrationsOrSubfolders()
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
            ;

            TestRun testRun = helper.RunEventsOnTest(new NumberOfFoldersWithoutRegistrationsOrSubfolders());

            testRun.Results.First().Message.Should().Be("2");
        }
    }
}
