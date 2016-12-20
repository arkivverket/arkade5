using System.Linq;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Tests.Noark5;
using FluentAssertions;
using Xunit;


namespace Arkivverket.Arkade.Test.Tests.Noark5
{
    public class NumberOfCasePartsTests
    {
        [Fact]
        public void NumberOfCasePartsIsOne()
        {
            XmlElementHelper helper = new XmlElementHelper().Add("arkiv",
                new XmlElementHelper()
                    .Add("arkivdel",
                        new XmlElementHelper().Add("klassifikasjonssystem",
                            new XmlElementHelper().Add("klasse",
                                new XmlElementHelper().Add("mappe",
                                    new XmlElementHelper().Add("registrering",
                                        new XmlElementHelper().Add("sakspart",
                                        new XmlElementHelper().Add("sakspartID", "Sakspart1"))))))));

            TestRun testRun = helper.RunEventsOnTest(new NumberOfCaseParts());

            testRun.Results.First().Message.Should().Contain("1");
        }

        [Fact]
        public void NumberOfCasePartsIsTwo()
        {
            XmlElementHelper helper = new XmlElementHelper().Add("arkiv",
                new XmlElementHelper()
                    .Add("arkivdel",
                        new XmlElementHelper().Add("klassifikasjonssystem",
                            new XmlElementHelper().Add("klasse",
                                new XmlElementHelper().Add("mappe",
                                    new XmlElementHelper().Add("registrering",
                                        new XmlElementHelper().Add("sakspart",
                                        new XmlElementHelper().Add("sakspartID", "Sakspart1")))))))
                .Add("arkivdel",
                        new XmlElementHelper().Add("klassifikasjonssystem",
                            new XmlElementHelper().Add("klasse",
                                new XmlElementHelper().Add("mappe",
                                    new XmlElementHelper().Add("registrering",
                                        new XmlElementHelper().Add("systemID", "journpost57d6608569ed33.70652483"))))))
                .Add("arkivdel",
                        new XmlElementHelper().Add("klassifikasjonssystem",
                            new XmlElementHelper().Add("klasse",
                                new XmlElementHelper().Add("mappe",
                                    new XmlElementHelper().Add("registrering",
                                        new XmlElementHelper().Add("sakspart",
                                        new XmlElementHelper().Add("sakspartID", "Sakspart1")))))))
                );

            TestRun testRun = helper.RunEventsOnTest(new NumberOfCaseParts());

            testRun.Results.First().Message.Should().Contain("2");
        }
    }
}
