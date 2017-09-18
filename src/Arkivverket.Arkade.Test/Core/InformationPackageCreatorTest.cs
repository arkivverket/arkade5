using System;
using System.Collections.Generic;
using System.IO;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Test.Metadata;
using FluentAssertions;
using ICSharpCode.SharpZipLib.Tar;
using Xunit;

namespace Arkivverket.Arkade.Test.Core
{
    /// <summary>
    /// Integration test of package creation. Should possibly be moved to separate package to avoid slow down of test running. File operations are performed during testing.
    /// </summary>
    public class InformationPackageCreatorTest
    {
        private readonly string _workingDirectory = AppDomain.CurrentDomain.BaseDirectory + "\\..\\..\\TestData\\package-creation\\";
        private readonly Uuid _uuid = Uuid.Random();
        private readonly ArchiveMetadata _archiveMetadata = MetsCreatorTest.FakeArchiveMetadata();
        private readonly string _outputDirectory = AppDomain.CurrentDomain.BaseDirectory;

        [Fact]
        [Trait("Category", "Integration")]
        public void ShouldCreateSip()
        {
            Archive archive = new ArchiveBuilder().WithUuid(_uuid).WithWorkingDirectoryRoot(_workingDirectory).Build();

            string packageFilePath = new InformationPackageCreator().CreateSip(archive, _archiveMetadata, _outputDirectory);

            List<string> fileList = GetFileListFromArchive(packageFilePath);

            fileList.Count.Should().Be(8);
            fileList.Contains("content\\").Should().BeTrue();
            fileList.Contains("content\\arkivstruktur.xml").Should().BeTrue();
            fileList.Contains("content\\arkivuttrekk.xml").Should().BeTrue();
            fileList.Contains("content\\dokumenter\\").Should().BeTrue();
            fileList.Contains("content\\dokumenter\\5000000.pdf").Should().BeTrue();
            fileList.Contains("content\\dokumenter\\5000001.pdf").Should().BeTrue();
            fileList.Contains("descriptive_metadata\\").Should().BeTrue();
            fileList.Contains("administrative_metadata\\").Should().BeTrue();

            // sip should not contain these files
            fileList.Contains("administrative_metadata\\repository_operations\\").Should().BeFalse();
            fileList.Contains("administrative_metadata\\repository_operations\\arkade-log.xml").Should().BeFalse();
            fileList.Contains("administrative_metadata\\repository_operations\\report.html").Should().BeFalse();
            fileList.Contains("descriptive_metadata\\ead.xml").Should().BeFalse();
            fileList.Contains("descriptive_metadata\\eac-cpf.xml").Should().BeFalse();
        }

        [Fact]
        [Trait("Category", "Integration")]
        public void ShouldCreateAip()
        {
            Uuid uuid = Uuid.Random();
            Archive archive = new ArchiveBuilder().WithUuid(uuid).WithWorkingDirectoryRoot(_workingDirectory).Build();

            string packageFilePath = new InformationPackageCreator().CreateAip(archive, _archiveMetadata, _outputDirectory);

            List<string> fileList = GetFileListFromArchive(packageFilePath);

            fileList.Count.Should().Be(13);
            fileList.Contains("content\\").Should().BeTrue();
            fileList.Contains("content\\arkivstruktur.xml").Should().BeTrue();
            fileList.Contains("content\\arkivuttrekk.xml").Should().BeTrue();
            fileList.Contains("content\\dokumenter\\").Should().BeTrue();
            fileList.Contains("content\\dokumenter\\5000000.pdf").Should().BeTrue();
            fileList.Contains("content\\dokumenter\\5000001.pdf").Should().BeTrue();
            fileList.Contains("descriptive_metadata\\").Should().BeTrue();
            fileList.Contains("administrative_metadata\\").Should().BeTrue();

            // additional files for aip
            fileList.Contains("administrative_metadata\\repository_operations\\").Should().BeTrue();
            fileList.Contains("administrative_metadata\\repository_operations\\arkade-log.xml").Should().BeTrue();
            fileList.Contains("administrative_metadata\\repository_operations\\report.html").Should().BeTrue();
            fileList.Contains("descriptive_metadata\\ead.xml").Should().BeTrue();
            fileList.Contains("descriptive_metadata\\eac-cpf.xml").Should().BeTrue();

        }

        private static List<string> GetFileListFromArchive(string targetFileName)
        {
            List<string> fileList = new List<string>();

            Stream inStream = File.OpenRead(targetFileName);
            TarArchive tarArchive = TarArchive.CreateInputTarArchive(inStream);
            tarArchive.ProgressMessageEvent += delegate(TarArchive archive1, TarEntry entry, string message)
            {
                // remove base path of all entries - 17 is length of 'package-creation\'
                fileList.Add(entry.Name.Substring(entry.Name.IndexOf("package-creation", StringComparison.Ordinal) + 17));
            };
            tarArchive.ListContents();
            return fileList;
        }
    }
}
