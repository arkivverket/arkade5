using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Logging;
using Arkivverket.Arkade.Core.Util;
using FluentAssertions;

namespace Arkivverket.Arkade.Core.V3Tests;

public class ArchiveFactoryTest
{
    private static readonly string TestDataDirectory = Path.Combine(Environment.CurrentDirectory, "TestData");
    private static readonly ArchiveFactory ArchiveFactory = new(new TarCompressionUtility(), new StatusEventHandler());

    [Fact]
    public void CreateTest()
    {
        ArkadeProcessingArea.Establish(TestDataDirectory);

        // SIARD

        var siardArchiveFile = new FileInfo(Path.Combine(TestDataDirectory, "Siard", "dbptk_produced.siard"));
        Archive siardArchive = ArchiveFactory.Create(siardArchiveFile, ArchiveType.Siard);
        siardArchive.GetType().Should().Be(typeof(SiardArchive));
        siardArchive.ProcessingDirectory.Delete(true);

        var siardArchiveInDiasTarFile = new FileInfo(Path.Combine(TestDataDirectory, "Siard", "Siard-utrekk_som_SIP", "e27fdbc9-93f7-46ad-865c-66699316400d.tar"));
        Archive siardArchiveFromDias = ArchiveFactory.Create(siardArchiveInDiasTarFile, ArchiveType.Siard);
        siardArchiveFromDias.GetType().Should().Be(typeof(SiardArchive));
        siardArchiveFromDias.ProcessingDirectory.Delete(true);

        // Noark 5

        var noark5ArchiveDirectory = new DirectoryInfo(Path.Combine(TestDataDirectory, "Noark5", "Noark5Archive"));
        Archive noark5Archive = ArchiveFactory.Create(noark5ArchiveDirectory, ArchiveType.Noark5);
        noark5Archive.GetType().Should().Be(typeof(Noark5Archive));
        noark5Archive.ProcessingDirectory.Delete(true);
        
        var noark5ArchiveInDiasTarFile = new FileInfo(Path.Combine(TestDataDirectory, "Noark5", "Noark5_liten_AIP", "c0ada300-e5f2-48f2-8ad6-4303a28ff2d2.tar"));
        Archive noark5ArchiveFromDias = ArchiveFactory.Create(noark5ArchiveInDiasTarFile, ArchiveType.Noark5);
        noark5ArchiveFromDias.GetType().Should().Be(typeof(Noark5Archive));
        noark5ArchiveFromDias.ProcessingDirectory.Delete(true);

        // Noark 4

        var noark4ArchiveDirectory = new DirectoryInfo(Path.Combine(TestDataDirectory, "Noark5", "Noark5Archive"));
        Archive noark4Archive = ArchiveFactory.Create(noark4ArchiveDirectory, ArchiveType.Noark4);
        noark4Archive.GetType().Should().Be(typeof(Noark4Archive));
        noark4Archive.ProcessingDirectory.Delete(true);

        var noark4ArchiveInDiasTarFile = new DirectoryInfo(TestDataDirectory);
        Archive noark4ArchiveFromDias = ArchiveFactory.Create(noark4ArchiveInDiasTarFile, ArchiveType.Noark4);
        noark4ArchiveFromDias.GetType().Should().Be(typeof(Noark4Archive));
        noark4ArchiveFromDias.ProcessingDirectory.Delete(true);

        // Noark 3

        var noark3ArchiveDirectory = new DirectoryInfo(TestDataDirectory);
        Archive noark3Archive = ArchiveFactory.Create(noark3ArchiveDirectory, ArchiveType.Noark3);
        noark3Archive.GetType().Should().Be(typeof(AddmlArchive));
        noark3Archive.ProcessingDirectory.Delete(true);

        var noark3ArchiveInDiasTarFile = new DirectoryInfo(TestDataDirectory);
        Archive noark3ArchiveFromDias = ArchiveFactory.Create(noark3ArchiveInDiasTarFile, ArchiveType.Noark3);
        noark3ArchiveFromDias.GetType().Should().Be(typeof(AddmlArchive));
        noark3ArchiveFromDias.ProcessingDirectory.Delete(true);

        // Fagsystem

        var fagsystemArchiveDirectory = new DirectoryInfo(TestDataDirectory);
        Archive fagsystemArchive = ArchiveFactory.Create(fagsystemArchiveDirectory, ArchiveType.Fagsystem);
        fagsystemArchive.GetType().Should().Be(typeof(AddmlArchive));
        fagsystemArchive.ProcessingDirectory.Delete(true);

        var fagsystemArchiveInDiasTarFile = new DirectoryInfo(TestDataDirectory);
        Archive fagsystemArchiveFromDias = ArchiveFactory.Create(fagsystemArchiveInDiasTarFile, ArchiveType.Fagsystem);
        fagsystemArchiveFromDias.GetType().Should().Be(typeof(AddmlArchive));
        fagsystemArchiveFromDias.ProcessingDirectory.Delete(true);

        ArkadeProcessingArea.Destroy(); // Release resources so that this can be destroyed.
    }
}
