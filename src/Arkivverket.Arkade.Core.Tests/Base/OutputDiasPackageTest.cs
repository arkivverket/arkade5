using System.IO;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Tests.UnitTestUtilities;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Core.Tests.Base;

public class OutputDiasPackageTest(TestSessionLifeTimeFilesFixture fixture)
{
    [Fact]
    public void MetadataIsUpdatedWithUuidFromOutputDiasPackage()
    {
        var archiveMetadata = new ArchiveMetadata();
        archiveMetadata.Id.Should().BeNull();

        var outputDiasPackage = new OutputDiasPackage(PackageType.SubmissionInformationPackage,
            archiveMetadata, fixture.CreateIsolatedDirectory<OutputDiasPackageTest>());

        outputDiasPackage.ArchiveMetadata.Id.Should().Be(outputDiasPackage.Id.ToString());
    }

    [Fact]
    public void MetadataIsUpdatedWithPackageTypeFromOutputDiasPackage()
    {
        var archiveMetadata = new ArchiveMetadata { PackageType = PackageType.SubmissionInformationPackage };

        var outputDiasPackage = new OutputDiasPackage(PackageType.ArchivalInformationPackage,
            archiveMetadata, fixture.CreateIsolatedDirectory<OutputDiasPackageTest>());

        outputDiasPackage.ArchiveMetadata.PackageType.Should().Be(PackageType.ArchivalInformationPackage);
    }

    [Fact]
    public void EachOutputDiasPackageGetsAFreshUuid()
    {
        DirectoryInfo location = fixture.CreateIsolatedDirectory<OutputDiasPackageTest>();

        var packageA = new OutputDiasPackage(PackageType.SubmissionInformationPackage, new ArchiveMetadata(), location);
        var packageB = new OutputDiasPackage(PackageType.SubmissionInformationPackage, new ArchiveMetadata(), location);

        packageA.Id.Should().NotBe(packageB.Id);
    }
}
