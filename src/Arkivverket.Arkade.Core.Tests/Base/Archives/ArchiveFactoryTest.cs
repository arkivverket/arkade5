using System.IO;
using System.Runtime.CompilerServices;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Base.Archives;
using Arkivverket.Arkade.Core.Logging;
using Arkivverket.Arkade.Core.Tests.UnitTestUtilities;
using Arkivverket.Arkade.Core.Util;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Core.Tests.Base.Archives;

public class ArchiveFactoryTest(TestSessionLifeTimeFilesFixture fixture)
{
    private static readonly ArchiveFactory ArchiveFactory = new(new TarCompressionUtility(), new StatusEventHandler());

    [Fact]
    public void CreateSiardArchiveTest()
    {
        PrepareForTemporaryFiles();
        var siardArchiveFile = new FileInfo(TestData.DirectoryPath(
            "Archives", "Siard", "extraction", "dbptk.siard"));
        Archive siardArchive = ArchiveFactory.Create(siardArchiveFile, ArchiveType.Siard);
        siardArchive.GetType().Should().Be(typeof(SiardArchive));
        siardArchive.Content.GetType().Should().Be(typeof(FileArchiveContent));
        siardArchive.ArchiveType.Should().Be(ArchiveType.Siard);
    }

    [Fact]
    public void CreateSiardArchiveFromDiasTest()
    {
        PrepareForTemporaryFiles();
        var siardArchiveInDiasTarFile = new FileInfo(TestData.DirectoryPath(
            "Archives", "Siard", "diasPackage", "841c0a18-7308-4407-b421-3efaa420d891.tar"));
        Archive siardArchiveFromDias = ArchiveFactory.Create(siardArchiveInDiasTarFile, ArchiveType.Siard);
        siardArchiveFromDias.GetType().Should().Be(typeof(SiardArchive));
        siardArchiveFromDias.Content.GetType().Should().Be(typeof(DirectoryArchiveContent));
        siardArchiveFromDias.ArchiveType.Should().Be(ArchiveType.Siard);
    }

    [Fact]
    public void CreateNoark5ArchiveTest()
    {
        PrepareForTemporaryFiles();
        var noark5ArchiveDirectory = new DirectoryInfo(TestData.DirectoryPath(
            "Archives", "Noark5", "extraction"));
        Archive noark5Archive = ArchiveFactory.Create(noark5ArchiveDirectory, ArchiveType.Noark5);
        noark5Archive.GetType().Should().Be(typeof(Noark5Archive));
        noark5Archive.Content.GetType().Should().Be(typeof(DirectoryArchiveContent));
        noark5Archive.ArchiveType.Should().Be(ArchiveType.Noark5);
    }

    [Fact]
    public void CreateNoark5ArchiveFromDiasTest()
    {
        PrepareForTemporaryFiles();
        var noark5ArchiveInDiasTarFile = new FileInfo(TestData.DirectoryPath(
            "Archives", "Noark5", "diasPackage", "4b73981c-1fab-4d4c-91e7-fcc6a3bc057f.tar"));
        Archive noark5ArchiveFromDias = ArchiveFactory.Create(noark5ArchiveInDiasTarFile, ArchiveType.Noark5);
        noark5ArchiveFromDias.GetType().Should().Be(typeof(Noark5Archive));
        noark5ArchiveFromDias.Content.GetType().Should().Be(typeof(DirectoryArchiveContent));
        ((DirectoryArchiveContent)noark5ArchiveFromDias.Content).GetDirectory("dokumenter").Should().BeNull(); // tar-ed
        noark5ArchiveFromDias.ArchiveType.Should().Be(ArchiveType.Noark5);
    }

    [Fact]
    public void CreateNoark4ArchiveTest()
    {
        PrepareForTemporaryFiles();
        var noark4ArchiveDirectory = new DirectoryInfo(TestData.DirectoryPath(
            "Archives", "Noark4", "extraction"));
        Archive noark4Archive = ArchiveFactory.Create(noark4ArchiveDirectory, ArchiveType.Noark4);
        noark4Archive.GetType().Should().Be(typeof(Noark4Archive));
        noark4Archive.Content.GetType().Should().Be(typeof(DirectoryArchiveContent));
        noark4Archive.ArchiveType.Should().Be(ArchiveType.Noark4);
    }

    [Fact]
    public void CreateNoark4ArchiveFromDiasTest()
    {
        PrepareForTemporaryFiles();
        var noark4ArchiveInDiasTarFile = new FileInfo(TestData.DirectoryPath(
            "Archives", "Noark4", "diasPackage", "ffb1fda0-5b13-478f-9e4a-d68e8a944399.tar"));
        Archive noark4ArchiveFromDias = ArchiveFactory.Create(noark4ArchiveInDiasTarFile, ArchiveType.Noark4);
        noark4ArchiveFromDias.GetType().Should().Be(typeof(Noark4Archive));
        noark4ArchiveFromDias.Content.GetType().Should().Be(typeof(DirectoryArchiveContent));
        noark4ArchiveFromDias.ArchiveType.Should().Be(ArchiveType.Noark4);
    }

    [Fact]
    public void CreateNoark3ArchiveTest()
    {
        PrepareForTemporaryFiles();
        var noark3ArchiveDirectory = new DirectoryInfo(TestData.DirectoryPath(
            "Archives", "Noark3", "extraction"));
        Archive noark3Archive = ArchiveFactory.Create(noark3ArchiveDirectory, ArchiveType.Noark3);
        noark3Archive.GetType().Should().Be(typeof(Noark3Archive));
        noark3Archive.Content.GetType().Should().Be(typeof(DirectoryArchiveContent));
        noark3Archive.ArchiveType.Should().Be(ArchiveType.Noark3);
    }

    [Fact]
    public void CreateNoark3ArchiveFromDiasTest()
    {
        PrepareForTemporaryFiles();
        var noark3ArchiveInDiasTarFile = new FileInfo(TestData.DirectoryPath(
            "Archives", "Noark3", "diasPackage", "8851c420-80e0-4681-b838-8eeb542d46f1.tar"));
        Archive noark3ArchiveFromDias = ArchiveFactory.Create(noark3ArchiveInDiasTarFile, ArchiveType.Noark3);
        noark3ArchiveFromDias.GetType().Should().Be(typeof(Noark3Archive));
        noark3ArchiveFromDias.Content.GetType().Should().Be(typeof(DirectoryArchiveContent));
        noark3ArchiveFromDias.ArchiveType.Should().Be(ArchiveType.Noark3);
    }

    [Fact]
    public void CreateSpecializedSystemArchiveTest()
    {
        PrepareForTemporaryFiles();
        var specializedSystemArchiveDirectory = new DirectoryInfo(TestData.DirectoryPath(
            "Archives", "SpecializedSystem", "extraction"));
        Archive specializedSystemArchive =
            ArchiveFactory.Create(specializedSystemArchiveDirectory, ArchiveType.SpecializedSystem);
        specializedSystemArchive.GetType().Should().Be(typeof(SpecializedSystemArchive));
        specializedSystemArchive.Content.GetType().Should().Be(typeof(DirectoryArchiveContent));
        specializedSystemArchive.ArchiveType.Should().Be(ArchiveType.SpecializedSystem);
    }

    [Fact]
    public void CreateSpecializedSystemArchiveFromDiasTest()
    {
        PrepareForTemporaryFiles();
        
        var specializedSystemArchiveInDiasTarFile = new FileInfo(TestData.DirectoryPath(
            "Archives", "SpecializedSystem", "diasPackage", "bf193afe-4483-4481-b457-e9ba4f19681c.tar"));
        Archive specializedSystemArchiveFromDias =
            ArchiveFactory.Create(specializedSystemArchiveInDiasTarFile, ArchiveType.SpecializedSystem);
        specializedSystemArchiveFromDias.GetType().Should().Be(typeof(SpecializedSystemArchive));
        specializedSystemArchiveFromDias.Content.GetType().Should().Be(typeof(DirectoryArchiveContent));
        specializedSystemArchiveFromDias.ArchiveType.Should().Be(ArchiveType.SpecializedSystem);
    }

    private void PrepareForTemporaryFiles([CallerMemberName] string testName = null)
    {
        string isolatedTemporaryDirectoryPath = fixture.CreateIsolatedDirectory<ArchiveFactoryTest>(testName).FullName;
        
        ArkadeProcessingArea.Establish(isolatedTemporaryDirectoryPath);
    }
}
