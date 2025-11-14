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

        ValidateArchiveCreatedFromExtraction(ArchiveType.Siard, typeof(SiardArchive), "Siard", "dbptk_produced.siard");
        ValidateArchiveCreatedFromExtraction(ArchiveType.Noark5, typeof(Noark5Archive), "Noark5", "Noark5Archive");
        ValidateArchiveCreatedFromExtraction(ArchiveType.Noark4, typeof(Noark4Archive));
        ValidateArchiveCreatedFromExtraction(ArchiveType.Noark3, typeof(AddmlArchive));
        ValidateArchiveCreatedFromExtraction(ArchiveType.Fagsystem, typeof(AddmlArchive));

        ValidateArchiveCreatedFromDiasPackage(ArchiveType.Siard, typeof(SiardArchive),
            "Siard", "Siard-utrekk_som_SIP", "e27fdbc9-93f7-46ad-865c-66699316400d.tar");
        ValidateArchiveCreatedFromDiasPackage(ArchiveType.Noark5, typeof(Noark5Archive),
            "Noark5", "Noark5_liten_AIP", "c0ada300-e5f2-48f2-8ad6-4303a28ff2d2.tar");
        // ValidateArchiveCreatedFromDiasPackage(ArchiveType.Noark4, typeof(Noark4Archive)); // Provide test-data (SIP/AIP)
        // ValidateArchiveCreatedFromDiasPackage(ArchiveType.Noark3, typeof(AddmlArchive)); // Provide test-data (SIP/AIP)
        // ValidateArchiveCreatedFromDiasPackage(ArchiveType.Fagsystem, typeof(AddmlArchive)); // Provide test-data (SIP/AIP)

        // ArkadeProcessingArea.Destroy(); // Release resources so that this can be destroyed.
    }
    
    private static void ValidateArchiveCreatedFromExtraction(ArchiveType archiveType, Type expectedType, params string[] testDataPathSegments)
    {
        string testDataPath = Path.Combine(TestDataDirectory, Path.Combine(testDataPathSegments));

        Archive archive = Path.HasExtension(testDataPath)
            ? ArchiveFactory.Create(new FileInfo(testDataPath), archiveType)
            : ArchiveFactory.Create(new DirectoryInfo(testDataPath), archiveType);

        archive.GetType().Should().Be(expectedType);
        archive.ArchiveType.Should().Be(archiveType); // Get rid of
        archive.InputDiasPackage.Should().BeNull();
    }

    private static void ValidateArchiveCreatedFromDiasPackage(ArchiveType archiveType, Type expectedType, params string[] testDataPathSegments)
    {
        string testDataPath = Path.Combine(TestDataDirectory, Path.Combine(testDataPathSegments));
    
        Archive archive;
        if (Path.HasExtension(testDataPath))
        {
            archive = ArchiveFactory.Create(new FileInfo(testDataPath), archiveType);
        }
        else
        {
            archive = ArchiveFactory.Create(new DirectoryInfo(testDataPath), archiveType);
        }

        archive.GetType().Should().Be(expectedType);
        archive.ArchiveType.Should().Be(archiveType); // Get rid of
        archive.InputDiasPackage.Should().NotBeNull();
    }
}