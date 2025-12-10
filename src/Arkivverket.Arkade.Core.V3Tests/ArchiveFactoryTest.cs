using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Base.Archives;
using Arkivverket.Arkade.Core.Logging;
using Arkivverket.Arkade.Core.Util;
using FluentAssertions;

namespace Arkivverket.Arkade.Core.V3Tests;

public class ArchiveFactoryTest : IDisposable
{
    private static readonly string TestDataDirectory =
        Path.Combine(Environment.CurrentDirectory, "TestData");

    private static readonly ArchiveFactory ArchiveFactory = new(new TarCompressionUtility(), new StatusEventHandler());

    public ArchiveFactoryTest() => ArkadeProcessingArea.Establish(TestDataDirectory);
    public void Dispose() => ArkadeProcessingArea.Destroy();

    [Fact]
    public void CreateSiardArchiveTest()
    {
        var siardArchiveFile = new FileInfo(Path.Combine(TestDataDirectory,
            "Archives", "Siard", "extraction", "dbptk.siard"));
        Archive siardArchive = ArchiveFactory.Create(siardArchiveFile, ArchiveType.Siard);
        siardArchive.GetType().Should().Be(typeof(SiardArchive));
        siardArchive.Content.GetType().Should().Be(typeof(FileArchiveContent));
        siardArchive.ArchiveType.Should().Be(ArchiveType.Siard);
        siardArchive.ProcessingDirectory.Delete(true);
    }

    [Fact]
    public void CreateSiardArchiveFromDiasTest()
    {
        var siardArchiveInDiasTarFile = new FileInfo(Path.Combine(TestDataDirectory,
            "Archives", "Siard", "diasPackage", "841c0a18-7308-4407-b421-3efaa420d891.tar"));
        Archive siardArchiveFromDias = ArchiveFactory.Create(siardArchiveInDiasTarFile, ArchiveType.Siard);
        siardArchiveFromDias.GetType().Should().Be(typeof(SiardArchive));
        siardArchiveFromDias.Content.GetType().Should().Be(typeof(DirectoryArchiveContent));
        siardArchiveFromDias.ArchiveType.Should().Be(ArchiveType.Siard);
        siardArchiveFromDias.ProcessingDirectory.Delete(true);
    }

    [Fact]
    public void CreateNoark5ArchiveTest()
    {
        var noark5ArchiveDirectory = new DirectoryInfo(Path.Combine(TestDataDirectory,
            "Archives", "Noark5", "extraction"));
        Archive noark5Archive = ArchiveFactory.Create(noark5ArchiveDirectory, ArchiveType.Noark5);
        noark5Archive.GetType().Should().Be(typeof(Noark5Archive));
        noark5Archive.Content.GetType().Should().Be(typeof(DirectoryArchiveContent));
        noark5Archive.ArchiveType.Should().Be(ArchiveType.Noark5);
        noark5Archive.ProcessingDirectory.Delete(true);
    }

    [Fact]
    public void CreateNoark5ArchiveFromDiasTest()
    {
        var noark5ArchiveInDiasTarFile = new FileInfo(Path.Combine(TestDataDirectory,
            "Archives", "Noark5", "diasPackage", "4b73981c-1fab-4d4c-91e7-fcc6a3bc057f.tar"));
        Archive noark5ArchiveFromDias = ArchiveFactory.Create(noark5ArchiveInDiasTarFile, ArchiveType.Noark5);
        noark5ArchiveFromDias.GetType().Should().Be(typeof(Noark5Archive));
        noark5ArchiveFromDias.Content.GetType().Should().Be(typeof(DirectoryArchiveContent));
        noark5ArchiveFromDias.ArchiveType.Should().Be(ArchiveType.Noark5);
        noark5ArchiveFromDias.ProcessingDirectory.Delete(true);
    }

    [Fact]
    public void CreateNoark4ArchiveTest()
    {
        var noark4ArchiveDirectory = new DirectoryInfo(Path.Combine(TestDataDirectory,
            "Archives", "Noark4", "extraction"));
        Archive noark4Archive = ArchiveFactory.Create(noark4ArchiveDirectory, ArchiveType.Noark4);
        noark4Archive.GetType().Should().Be(typeof(Noark4Archive));
        noark4Archive.Content.GetType().Should().Be(typeof(DirectoryArchiveContent));
        noark4Archive.ArchiveType.Should().Be(ArchiveType.Noark4);
        noark4Archive.ProcessingDirectory.Delete(true);
    }

    [Fact]
    public void CreateNoark4ArchiveFromDiasTest()
    {
        var noark4ArchiveInDiasTarFile = new FileInfo(Path.Combine(TestDataDirectory,
            "Archives", "Noark4", "diasPackage", "ffb1fda0-5b13-478f-9e4a-d68e8a944399.tar"));
        Archive noark4ArchiveFromDias = ArchiveFactory.Create(noark4ArchiveInDiasTarFile, ArchiveType.Noark4);
        noark4ArchiveFromDias.GetType().Should().Be(typeof(Noark4Archive));
        noark4ArchiveFromDias.Content.GetType().Should().Be(typeof(DirectoryArchiveContent));
        noark4ArchiveFromDias.ArchiveType.Should().Be(ArchiveType.Noark4);
        noark4ArchiveFromDias.ProcessingDirectory.Delete(true);
    }

    [Fact]
    public void CreateNoark3ArchiveTest()
    {
        var noark3ArchiveDirectory = new DirectoryInfo(Path.Combine(TestDataDirectory,
            "Archives", "Noark3", "extraction"));
        Archive noark3Archive = ArchiveFactory.Create(noark3ArchiveDirectory, ArchiveType.Noark3);
        noark3Archive.GetType().Should().Be(typeof(Noark3Archive));
        noark3Archive.Content.GetType().Should().Be(typeof(DirectoryArchiveContent));
        noark3Archive.ArchiveType.Should().Be(ArchiveType.Noark3);
        noark3Archive.ProcessingDirectory.Delete(true);
    }

    [Fact]
    public void CreateNoark3ArchiveFromDiasTest()
    {
        var noark3ArchiveInDiasTarFile = new FileInfo(Path.Combine(TestDataDirectory,
            "Archives", "Noark3", "diasPackage", "8851c420-80e0-4681-b838-8eeb542d46f1.tar"));
        Archive noark3ArchiveFromDias = ArchiveFactory.Create(noark3ArchiveInDiasTarFile, ArchiveType.Noark3);
        noark3ArchiveFromDias.GetType().Should().Be(typeof(Noark3Archive));
        noark3ArchiveFromDias.Content.GetType().Should().Be(typeof(DirectoryArchiveContent));
        noark3ArchiveFromDias.ArchiveType.Should().Be(ArchiveType.Noark3);
        noark3ArchiveFromDias.ProcessingDirectory.Delete(true);
    }

    [Fact]
    public void CreateSpecializedSystemArchiveTest()
    {
        var specializedSystemArchiveDirectory = new DirectoryInfo(Path.Combine(TestDataDirectory,
            "Archives", "SpecializedSystem", "extraction"));
        Archive specializedSystemArchive =
            ArchiveFactory.Create(specializedSystemArchiveDirectory, ArchiveType.SpecializedSystem);
        specializedSystemArchive.GetType().Should().Be(typeof(SpecializedSystemArchive));
        specializedSystemArchive.Content.GetType().Should().Be(typeof(DirectoryArchiveContent));
        specializedSystemArchive.ArchiveType.Should().Be(ArchiveType.SpecializedSystem);
        specializedSystemArchive.ProcessingDirectory.Delete(true);
    }

    [Fact]
    public void CreateSpecializedSystemArchiveFromDiasTest()
    {
        var specializedSystemArchiveInDiasTarFile = new FileInfo(Path.Combine(TestDataDirectory,
            "Archives", "SpecializedSystem", "diasPackage", "bf193afe-4483-4481-b457-e9ba4f19681c.tar"));
        Archive specializedSystemArchiveFromDias =
            ArchiveFactory.Create(specializedSystemArchiveInDiasTarFile, ArchiveType.SpecializedSystem);
        specializedSystemArchiveFromDias.GetType().Should().Be(typeof(SpecializedSystemArchive));
        specializedSystemArchiveFromDias.Content.GetType().Should().Be(typeof(DirectoryArchiveContent));
        specializedSystemArchiveFromDias.ArchiveType.Should().Be(ArchiveType.SpecializedSystem);
        specializedSystemArchiveFromDias.ProcessingDirectory.Delete(true);
    }
}
