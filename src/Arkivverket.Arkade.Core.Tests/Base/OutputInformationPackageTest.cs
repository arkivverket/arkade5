using System.IO;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Languages;
using Arkivverket.Arkade.Core.Logging;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Core.Tests.Base;

public class OutputInformationPackageTest
{

    [Fact]
    public void MetadataShouldBeUpdatedWithUuidFromOutPutInformationPackage()
    {
        var workingDirectory = new WorkingDirectory(new DirectoryInfo(Path.GetRandomFileName()), new DirectoryInfo(Path.GetRandomFileName()));
        var archive = new Archive(ArchiveType.Noark5, Uuid.Random(), workingDirectory, new StatusEventHandler());

        var archiveMetadata = new ArchiveMetadata();

        archiveMetadata.Id.Should().BeNull();

        var outputInformationPackage = new OutputInformationPackage(PackageType.SubmissionInformationPackage, archive, archiveMetadata, SupportedLanguage.en);
        
        outputInformationPackage.ArchiveMetadata.Id.Should().BeEquivalentTo($"UUID:{outputInformationPackage.Uuid}");
    }
}
