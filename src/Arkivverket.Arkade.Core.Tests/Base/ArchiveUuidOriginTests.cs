using System;
using System.Runtime.CompilerServices;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Base.Archives;
using Arkivverket.Arkade.Core.Logging;
using Arkivverket.Arkade.Core.Tests.UnitTestUtilities;
using Arkivverket.Arkade.Core.Util;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Core.Tests.Base;

// A UUID identifies a DIAS package, not an archive extraction: identity is read from a DIAS
// package at load, never invented for other input — and output packages mint their own.
public class ArchiveUuidOriginTests(TestSessionLifeTimeFilesFixture fixture)
{
    private static readonly ArchiveFactory ArchiveFactory = new(new TarCompressionUtility(), new StatusEventHandler());

    [Fact]
    public void NoPackageIdentityWhenInputIsArchiveContentDirectory()
    {
        PrepareForTemporaryFiles();
        var contentDirectory = TestData.Directory("UUID-origin-control", "Noark5-extract", "content");

        Archive archive = ArchiveFactory.Create(contentDirectory, ArchiveType.Noark5);

        archive.InputDiasPackage.Should().BeNull();
    }

    [Fact]
    public void NoPackageIdentityWhenInputIsSiardArchiveFile()
    {
        PrepareForTemporaryFiles();
        var siardFile = TestData.File("UUID-origin-control", "Siard-extract.siard");

        Archive archive = ArchiveFactory.Create(siardFile, ArchiveType.Siard);

        archive.InputDiasPackage.Should().BeNull();
    }

    [Fact]
    public void InputPackageIdIsReadFromDiasTarFileName()
    {
        PrepareForTemporaryFiles();
        var diasTarFile = TestData.File("UUID-origin-control", "258e3353-cef2-407f-92ac-264ad887527b.tar");

        Archive archive = ArchiveFactory.Create(diasTarFile, ArchiveType.Noark5);

        archive.InputDiasPackage.Id.ToString().Should().Be("258e3353-cef2-407f-92ac-264ad887527b");
    }

    [Fact]
    public void DiasTarFileWithoutUuidFileNameIsRejectedAtLoad()
    {
        PrepareForTemporaryFiles();
        var diasTarFile = TestData.File("UUID-origin-control", "invalid-uuid.tar");

        Action loadingDiasTarFile = () => ArchiveFactory.Create(diasTarFile, ArchiveType.Noark5);

        loadingDiasTarFile.Should().Throw<ArkadeException>().WithMessage("*invalid-uuid.tar*");
    }

    [Fact]
    public void OutputPackageIdIsNotInheritedFromInputPackage()
    {
        PrepareForTemporaryFiles();
        var diasTarFile = TestData.File("UUID-origin-control", "258e3353-cef2-407f-92ac-264ad887527b.tar");
        Archive archive = ArchiveFactory.Create(diasTarFile, ArchiveType.Noark5);

        archive.OutputDiasPackage = new OutputDiasPackage(
            PackageType.ArchivalInformationPackage, new ArchiveMetadata(), archive.ProcessingDirectory);

        archive.OutputDiasPackage.Id.Should().NotBeNull();
        archive.OutputDiasPackage.Id.Should().NotBe(archive.InputDiasPackage.Id);
    }

    private void PrepareForTemporaryFiles([CallerMemberName] string testName = null)
    {
        string isolatedTemporaryDirectoryPath =
            fixture.CreateIsolatedDirectory<ArchiveUuidOriginTests>(testName).FullName;

        ArkadeProcessingArea.Establish(isolatedTemporaryDirectoryPath);
    }
}
