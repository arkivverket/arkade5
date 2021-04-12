using System;
using System.IO;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Testing.Noark5.Structure;
using Arkivverket.Arkade.Core.Tests.Base;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Core.Tests.Testing.Noark5.Structure
{
    public class N5_28_ValidateNumberOfDocumentfilesTest : LanguageDependentTest
    {
        [Fact]
        public void DocumentedAndActualFileCountMatch()
        {
            string workingDirectory =
                $"{AppDomain.CurrentDomain.BaseDirectory}\\TestData\\Noark5\\DocumentfilesControl\\CountMatch";

            TestRun testRun = CreateTestRun(workingDirectory);

            testRun.Results.Count.Should().Be(2);
            testRun.Results.Should().Contain(r => r.Message.Equals("Oppgitt antall dokumentfiler: 2"));
            testRun.Results.Should().Contain(r => r.Message.Equals("Antall dokumentfiler funnet: 2"));
        }

        [Fact]
        public void DocumentedAndActualFileCountMisMatch()
        {
            string workingDirectory =
                $"{AppDomain.CurrentDomain.BaseDirectory}\\TestData\\Noark5\\DocumentfilesControl\\CountMisMatch";

            TestRun testRun = CreateTestRun(workingDirectory);

            testRun.Results.Count.Should().Be(3);
            testRun.Results.Should().Contain(r => r.Message.Equals("Oppgitt antall dokumentfiler: 2"));
            testRun.Results.Should().Contain(r => r.Message.Equals("Antall dokumentfiler funnet: 1"));
            testRun.Results.Should().Contain(r =>
                r.Message.Equals("Det er ikke samsvar mellom oppgitt antall og faktisk antall dokumentfiler") &&
                r.IsError());
        }

        [Fact]
        public void DocumentedFileCountIsMissing()
        {
            string workingDirectory =
                $"{AppDomain.CurrentDomain.BaseDirectory}\\TestData\\Noark5\\DocumentfilesControl\\MissingDocumentation";

            TestRun testRun = CreateTestRun(workingDirectory);

            testRun.Results.Count.Should().Be(2);
            testRun.Results.Should().Contain(r => r.Message.Equals("Antall dokumentfiler funnet: 2"));
            testRun.Results.Should()
                .Contain(r => r.Message.Equals("Antall dokumentfiler ble ikke funnet oppgitt") && r.IsError());
        }

        [Fact]
        public void ThereIsNoFiles()
        {
            string workingDirectory =
                $"{AppDomain.CurrentDomain.BaseDirectory}\\TestData\\Noark5\\DocumentfilesControl\\NoFiles";

            File.Delete(workingDirectory + @"\content\dokumenter\directorykeeper.txt");

            TestRun testRun = CreateTestRun(workingDirectory);

            testRun.Results.Count.Should().Be(2);
            testRun.Results.Should().Contain(r => r.Message.Equals("Oppgitt antall dokumentfiler: 0"));
            testRun.Results.Should().Contain(r => r.Message.Equals("Ingen dokumentfiler funnet") && r.IsError());
        }

        [Fact]
        public void ThereIsNoFilesDirectory()
        {
            string workingDirectory =
                $"{AppDomain.CurrentDomain.BaseDirectory}\\TestData\\Noark5\\DocumentfilesControl\\NoFilesDirectory";

            TestRun testRun = CreateTestRun(workingDirectory);

            testRun.Results.Count.Should().Be(2);
            testRun.Results.Should().Contain(r => r.Message.Equals("Oppgitt antall dokumentfiler: 0"));
            testRun.Results.Should().Contain(r => r.Message.Equals("Ingen dokumentfilkatalog funnet") && r.IsError());
        }

        [Fact]
        public void DocumentedAndActualFileCountMatchWhenThereIsSubDirectories()
        {
            string workingDirectory =
                $"{AppDomain.CurrentDomain.BaseDirectory}\\TestData\\Noark5\\DocumentfilesControl\\Sub-directories";

            TestRun testRun = CreateTestRun(workingDirectory);

            testRun.Results.Count.Should().Be(2);
            testRun.Results.Should().Contain(r => r.Message.Equals("Oppgitt antall dokumentfiler: 3"));
            testRun.Results.Should().Contain(r => r.Message.Equals("Antall dokumentfiler funnet: 3"));
        }

        private static TestRun CreateTestRun(string workingDirectory)
        {
            Archive archive = new ArchiveBuilder()
                .WithArchiveType(ArchiveType.Noark5)
                .WithWorkingDirectoryRoot(workingDirectory)
                .WithWorkingDirectoryExternalContent(workingDirectory + "\\content")
                .Build();

            var validateNumberOfDocumentfiles = new N5_28_ValidateNumberOfDocumentfiles();

            validateNumberOfDocumentfiles.Test(archive);

            return validateNumberOfDocumentfiles.GetTestRun();
        }
    }
}
