using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Logging;
using Arkivverket.Arkade.Core.Util;
using FluentAssertions;

namespace Arkivverket.Arkade.Core.V3Tests;

public class ArchiveFactoryTest
{
    [Fact]
    public void CreateTest()
    {
        CallWith(new DirectoryInfo(""), ArchiveType.Siard).GetType().Should().Be(typeof(SiardArchive));
        CallWith(new DirectoryInfo(""), ArchiveType.Noark5).GetType().Should().Be(typeof(Noark5Archive));
        CallWith(new DirectoryInfo(""), ArchiveType.Noark4).GetType().Should().Be(typeof(Noark4Archive));
        CallWith(new DirectoryInfo(""), ArchiveType.Noark3).GetType().Should().Be(typeof(AddmlArchive));
        CallWith(new DirectoryInfo(""), ArchiveType.Fagsystem).GetType().Should().Be(typeof(AddmlArchive));
    }

    private static Archive CallWith(FileSystemInfo archiveSource, ArchiveType archiveType)
    {
        return new ArchiveFactory(new TarCompressionUtility(), new StatusEventHandler())
            .Create(archiveSource, archiveType);
    }
}
