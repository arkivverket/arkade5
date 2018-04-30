﻿using System;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Test.Core;
using Arkivverket.Arkade.Tests.Noark5;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Test.Tests.Noark5
{
    public class NumberOfRestrictionsTest
    {
        [Fact]
        public void HasSeverealRestrictionsOnSingleArchivePart()
        {
            XmlElementHelper helper = new XmlElementHelper()
                .Add("arkiv",
                    new XmlElementHelper()
                        .Add("arkivdel",
                            new XmlElementHelper()
                                .Add("systemID", "someArchivePartSystemId_1")
                                .Add("skjerming", string.Empty)
                                .Add("klassifikasjonssystem",
                                    new XmlElementHelper()
                                        .Add("klasse",
                                            new XmlElementHelper()
                                                .Add("skjerming", string.Empty)
                                                .Add("mappe",
                                                    new XmlElementHelper()
                                                        .Add("skjerming", string.Empty)
                                                        .Add("registrering",
                                                            new XmlElementHelper()
                                                                .Add("skjerming", string.Empty)
                                                                .Add("dokumentbeskrivelse",
                                                                    new XmlElementHelper()
                                                                        .Add("skjerming", string.Empty))))
                                                .Add("mappe",
                                                    new XmlElementHelper()
                                                        .Add("skjerming", string.Empty)
                                                        .Add("registrering",
                                                            new XmlElementHelper()
                                                                .Add("dokumentbeskrivelse",
                                                                    new XmlElementHelper()
                                                                        .Add("skjerming", string.Empty))))))));

            // Creating a test archive stating that it should contain restrictions:
            var testArchive = new ArchiveBuilder().WithArchiveType(ArchiveType.Noark5)
                .WithWorkingDirectoryRoot("TestData/Noark5/MetaDataTesting/BooleansTrue").Build();

            TestRun testRun = helper.RunEventsOnTest(new NumberOfRestrictions(testArchive));

            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Skjerminger i arkivdel - Antall: 1"
            ));
            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Skjerminger i klasse - Antall: 1"
            ));
            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Skjerminger i mappe - Antall: 2"
            ));
            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Skjerminger i registrering - Antall: 1"
            ));
            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Skjerminger i dokumentbeskrivelse - Antall: 2"
            ));
            testRun.Results.Count.Should().Be(5);
        }

        [Fact]
        public void HasSeverealRestrictionsOnSeveralArchiveParts()
        {
            XmlElementHelper helper = new XmlElementHelper()
                .Add("arkiv",
                    new XmlElementHelper()
                        .Add("arkivdel",
                            new XmlElementHelper()
                                .Add("systemID", "someArchivePartSystemId_1")
                                .Add("skjerming", string.Empty)
                                .Add("klassifikasjonssystem",
                                    new XmlElementHelper()
                                        .Add("klasse",
                                            new XmlElementHelper()
                                                .Add("skjerming", string.Empty)
                                                .Add("mappe",
                                                    new XmlElementHelper()
                                                        .Add("skjerming", string.Empty)
                                                        .Add("registrering",
                                                            new XmlElementHelper()
                                                                .Add("skjerming", string.Empty)
                                                                .Add("dokumentbeskrivelse",
                                                                    new XmlElementHelper()
                                                                        .Add("skjerming", string.Empty))))
                                                .Add("mappe",
                                                    new XmlElementHelper()
                                                        .Add("skjerming", string.Empty)
                                                        .Add("registrering",
                                                            new XmlElementHelper()
                                                                .Add("dokumentbeskrivelse",
                                                                    new XmlElementHelper()
                                                                        .Add("skjerming", string.Empty)))))))
                        .Add("arkivdel",
                            new XmlElementHelper()
                                .Add("systemID", "someArchivePartSystemId_2")
                                .Add("skjerming", string.Empty)
                                .Add("klassifikasjonssystem",
                                    new XmlElementHelper()
                                        .Add("klasse",
                                            new XmlElementHelper()
                                                .Add("skjerming", string.Empty)
                                                .Add("mappe",
                                                    new XmlElementHelper()
                                                        .Add("skjerming", string.Empty)
                                                        .Add("registrering",
                                                            new XmlElementHelper()
                                                                .Add("skjerming", string.Empty)
                                                                .Add("dokumentbeskrivelse",
                                                                    new XmlElementHelper()
                                                                        .Add("skjerming", string.Empty))))
                                                .Add("mappe",
                                                    new XmlElementHelper()
                                                        .Add("skjerming", string.Empty)
                                                        .Add("registrering",
                                                            new XmlElementHelper()
                                                                .Add("dokumentbeskrivelse",
                                                                    new XmlElementHelper()
                                                                        .Add("skjerming", string.Empty))))))));

            // Creating a test archive stating that it should contain restrictions:
            var testArchive = new ArchiveBuilder().WithArchiveType(ArchiveType.Noark5)
                .WithWorkingDirectoryRoot("TestData/Noark5/MetaDataTesting/BooleansTrue").Build();

            TestRun testRun = helper.RunEventsOnTest(new NumberOfRestrictions(testArchive));

            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Arkivdel (systemID): someArchivePartSystemId_1 - Skjerminger i arkivdel - Antall: 1"
            ));
            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Arkivdel (systemID): someArchivePartSystemId_1 - Skjerminger i klasse - Antall: 1"
            ));
            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Arkivdel (systemID): someArchivePartSystemId_1 - Skjerminger i mappe - Antall: 2"
            ));
            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Arkivdel (systemID): someArchivePartSystemId_1 - Skjerminger i registrering - Antall: 1"
            ));
            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Arkivdel (systemID): someArchivePartSystemId_1 - Skjerminger i dokumentbeskrivelse - Antall: 2"
            ));
            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Arkivdel (systemID): someArchivePartSystemId_2 - Skjerminger i arkivdel - Antall: 1"
            ));
            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Arkivdel (systemID): someArchivePartSystemId_2 - Skjerminger i klasse - Antall: 1"
            ));
            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Arkivdel (systemID): someArchivePartSystemId_2 - Skjerminger i mappe - Antall: 2"
            ));
            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Arkivdel (systemID): someArchivePartSystemId_2 - Skjerminger i registrering - Antall: 1"
            ));
            testRun.Results.Should().Contain(r => r.Message.Equals(
                "Arkivdel (systemID): someArchivePartSystemId_2 - Skjerminger i dokumentbeskrivelse - Antall: 2"
            ));
            testRun.Results.Count.Should().Be(10);
        }

        [Fact]
        public void ShouldRaiseWarningWithDocumentedRestrictionsFalseAndActualRestrictionsTrue()
        {
            XmlElementHelper helper = new XmlElementHelper()
                .Add("arkiv",
                    new XmlElementHelper()
                        .Add("arkivdel",
                            new XmlElementHelper()
                                .Add("systemID", "someArchivePartSystemId_1")
                                .Add("klassifikasjonssystem",
                                    new XmlElementHelper()
                                        .Add("klasse",
                                            new XmlElementHelper()
                                                .Add("mappe",
                                                    new XmlElementHelper()
                                                        .Add("registrering",
                                                            new XmlElementHelper()
                                                                .Add("dokumentbeskrivelse",
                                                                    new XmlElementHelper()
                                                                        .Add("skjerming", string.Empty))))))));


            // Creating a test archive stating that it should not contain any restrictions:
            var testArchive = new ArchiveBuilder().WithArchiveType(ArchiveType.Noark5)
                .WithWorkingDirectoryRoot("TestData/Noark5/MetaDataTesting/BooleansFalse").Build();

            TestRun testRun = helper.RunEventsOnTest(new NumberOfRestrictions(testArchive));

            testRun.Results.Should()
                .Contain(r => r.Message.Equals("Skjerminger i dokumentbeskrivelse - Antall: 1"));

            testRun.Results.Should().Contain(r =>
                r.IsError() && r.Message.Equals(
                    "Det er angitt at uttrekket ikke skal inneholde skjerminger, men skjerminger ble funnet"
                ) && r.Location.ToString().Equals("arkivuttrekk.xml"));

            testRun.Results.Count.Should().Be(2);
        }

        [Fact]
        public void ShouldRaiseWarningWithDocumentedRestrictionsTrueAndActualRestrictionsFalse()
        {
            XmlElementHelper helper = new XmlElementHelper()
                .Add("arkiv",
                    new XmlElementHelper()
                        .Add("arkivdel",
                            new XmlElementHelper()
                                .Add("systemID", "someArchivePartSystemId_1")
                                .Add("klassifikasjonssystem",
                                    new XmlElementHelper()
                                        .Add("klasse",
                                            new XmlElementHelper()
                                                .Add("mappe",
                                                    new XmlElementHelper()
                                                        .Add("registrering",
                                                            new XmlElementHelper()
                                                                .Add("dokumentbeskrivelse", string.Empty)))))));


            // Creating a test archive stating that it should contain restrictions:
            var testArchive = new ArchiveBuilder().WithArchiveType(ArchiveType.Noark5)
                .WithWorkingDirectoryRoot("TestData/Noark5/MetaDataTesting/BooleansTrue").Build();

            TestRun testRun = helper.RunEventsOnTest(new NumberOfRestrictions(testArchive));

            testRun.Results.Should().Contain(r =>
                r.IsError() && r.Message.Equals(
                    "Det er angitt at uttrekket skal inneholde skjerminger, men ingen skjerminger ble funnet"
                ) && r.Location.ToString().Equals("arkivuttrekk.xml"));

            testRun.Results.Count.Should().Be(1);
        }
    }
}
