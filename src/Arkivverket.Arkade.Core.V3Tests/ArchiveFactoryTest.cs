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

        ValidateArchiveCreated(ArchiveType.Siard, typeof(SiardArchive), "Siard", "dbptk_produced.siard");
        ValidateArchiveCreated(ArchiveType.Noark5, typeof(Noark5Archive), "Noark5", "Noark5Archive");
        ValidateArchiveCreated(ArchiveType.Noark4, typeof(Noark4Archive));
        ValidateArchiveCreated(ArchiveType.Noark3, typeof(AddmlArchive));
        ValidateArchiveCreated(ArchiveType.Fagsystem, typeof(AddmlArchive));
        
        ArkadeProcessingArea.Destroy();
    }

    private static void ValidateArchiveCreated(ArchiveType archiveType, Type expectedType, params string[] testDataPathSegments)
    {
        string testDataPath = Path.Combine(TestDataDirectory, Path.Combine(testDataPathSegments));

        Archive archive = Path.HasExtension(testDataPath)
            ? ArchiveFactory.Create(new FileInfo(testDataPath), archiveType)
            : ArchiveFactory.Create(new DirectoryInfo(testDataPath), archiveType);

        archive.GetType().Should().Be(expectedType);
    }
}
