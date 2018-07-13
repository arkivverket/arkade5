using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Tests.Metadata;
using FluentAssertions;
using ICSharpCode.SharpZipLib.Tar;
using Xunit;

namespace Arkivverket.Arkade.Core.Tests.Base
{
    /// <summary>
    /// Integration test of package creation. Should possibly be moved to separate package to avoid slow down of test running. File operations are performed during testing.
    /// </summary>
    public class InformationPackageCreatorTest
    {
        private readonly string _workingDirectory = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\TestData\\package-creation");
        private readonly Uuid _uuid = Uuid.Random();
        private readonly ArchiveMetadata _archiveMetadata = MetsCreatorTest.FakeArchiveMetadata();
        private readonly string _outputDirectory = AppDomain.CurrentDomain.BaseDirectory;

        [Fact]
        [Trait("Category", "Integration")]
        public void ShouldCreateSip() // TODO: Remove the created packages
        {
            Archive archive = new ArchiveBuilder().WithUuid(_uuid).WithWorkingDirectoryRoot(_workingDirectory).Build();

            string packageFilePath = new InformationPackageCreator().CreateSip(archive, _archiveMetadata, _outputDirectory);

            List<string> fileList = GetFileListFromArchive(packageFilePath);

            string rootDir = _uuid.GetValue() + "/";

            fileList.Count.Should().Be(9);
            fileList.Contains(rootDir).Should().BeTrue();
            fileList.Contains(rootDir + "content/").Should().BeTrue();
            fileList.Contains(rootDir + "content/arkivstruktur.xml").Should().BeTrue();
            fileList.Contains(rootDir + "content/arkivuttrekk.xml").Should().BeTrue();
            fileList.Contains(rootDir + "content/dokumenter/").Should().BeTrue();
            fileList.Contains(rootDir + "content/dokumenter/5000000.pdf").Should().BeTrue();
            fileList.Contains(rootDir + "content/dokumenter/5000001.pdf").Should().BeTrue();
            fileList.Contains(rootDir + "descriptive_metadata/").Should().BeTrue();
            fileList.Contains(rootDir + "administrative_metadata/").Should().BeTrue();

            // sip should not contain these files
            fileList.Contains(rootDir + "administrative_metadata/repository_operations/").Should().BeFalse();
            fileList.Contains(rootDir + "administrative_metadata/repository_operations/arkade-log.xml").Should().BeFalse();
            fileList.Contains(rootDir + "administrative_metadata/repository_operations/report.html").Should().BeFalse();
            fileList.Contains(rootDir + "descriptive_metadata/ead.xml").Should().BeFalse();
            fileList.Contains(rootDir + "descriptive_metadata/eac-cpf.xml").Should().BeFalse();
        }

        [Fact]
        [Trait("Category", "Integration")]
        public void ShouldCreateAip() // TODO: Remove the created packages
        {
            Uuid uuid = Uuid.Random();
            Archive archive = new ArchiveBuilder().WithUuid(uuid).WithWorkingDirectoryRoot(_workingDirectory).Build();

            string packageFilePath = new InformationPackageCreator().CreateAip(archive, _archiveMetadata, _outputDirectory);

            List<string> fileList = GetFileListFromArchive(packageFilePath);

            string rootDir = uuid.GetValue() + "/";

            fileList.Count.Should().Be(14);
            fileList.Contains(rootDir).Should().BeTrue();
            fileList.Contains(rootDir + "content/").Should().BeTrue();
            fileList.Contains(rootDir + "content/arkivstruktur.xml").Should().BeTrue();
            fileList.Contains(rootDir + "content/arkivuttrekk.xml").Should().BeTrue();
            fileList.Contains(rootDir + "content/dokumenter/").Should().BeTrue();
            fileList.Contains(rootDir + "content/dokumenter/5000000.pdf").Should().BeTrue();
            fileList.Contains(rootDir + "content/dokumenter/5000001.pdf").Should().BeTrue();
            fileList.Contains(rootDir + "descriptive_metadata/").Should().BeTrue();
            fileList.Contains(rootDir + "administrative_metadata/").Should().BeTrue();

            // additional files for aip
            fileList.Contains(rootDir + "administrative_metadata/repository_operations/").Should().BeTrue();
            fileList.Contains(rootDir + "administrative_metadata/repository_operations/arkade-log.xml").Should().BeTrue();
            fileList.Contains(rootDir + "administrative_metadata/repository_operations/report.html").Should().BeTrue();
            fileList.Contains(rootDir + "descriptive_metadata/ead.xml").Should().BeTrue();
            fileList.Contains(rootDir + "descriptive_metadata/eac-cpf.xml").Should().BeTrue();

        }

        private static List<string> GetFileListFromArchive(string targetFileName)
        {
            List<string> fileList = new List<string>();

            Stream inStream = File.OpenRead(targetFileName);
            TarArchive tarArchive = TarArchive.CreateInputTarArchive(inStream);
            tarArchive.ProgressMessageEvent += delegate(TarArchive archive1, TarEntry entry, string message)
            {
                fileList.Add(entry.Name);
            };
            tarArchive.ListContents();
            return fileList;
        }
    }
}
