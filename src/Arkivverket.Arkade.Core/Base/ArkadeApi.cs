using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Arkivverket.Arkade.Core.Base.Archives;
using Arkivverket.Arkade.Core.Base.Siard;
using Arkivverket.Arkade.Core.Identify;
using Arkivverket.Arkade.Core.Languages;
using Arkivverket.Arkade.Core.Logging;
using Arkivverket.Arkade.Core.Metadata;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Util;
using Arkivverket.Arkade.Core.Util.ArchiveFormatValidation;
using Arkivverket.Arkade.Core.Util.FileFormatIdentification;
using iText.Kernel.Geom;
using Serilog;
using Path = System.IO.Path;

namespace Arkivverket.Arkade.Core.Base
{

    /// <summary>
    /// Use this class for interacting with the Arkade Api when you are using Autofac. If you don't use Autofac, please use the Arkade class instead.
    /// </summary>
    public class ArkadeApi
    {
        private static readonly ILogger Log = Serilog.Log.ForContext(MethodBase.GetCurrentMethod()?.DeclaringType);

        private readonly IArchiveTypeIdentifier _archiveTypeIdentifier;
        private readonly IArchiveFormatValidator _archiveFormatValidator;
        private readonly IFileFormatIdentifier _fileFormatIdentifier;
        private readonly IFileFormatInfoFilesGenerator _fileFormatInfoGenerator;
        private readonly ISiardXmlTableReader _siardXmlTableReader;
        private readonly MetadataExampleGenerator _metadataExampleGenerator;

        public ArkadeApi(IArchiveTypeIdentifier archiveTypeIdentifier, IArchiveFormatValidator archiveFormatValidator,
            IFileFormatIdentifier fileFormatIdentifier, IFileFormatInfoFilesGenerator fileFormatInfoGenerator,
            ISiardXmlTableReader siardXmlTableReader, MetadataExampleGenerator metadataExampleGenerator)
        {
            _archiveTypeIdentifier = archiveTypeIdentifier;
            _archiveFormatValidator = archiveFormatValidator;
            _fileFormatIdentifier = fileFormatIdentifier;
            _fileFormatInfoGenerator = fileFormatInfoGenerator;
            _siardXmlTableReader = siardXmlTableReader;
            _metadataExampleGenerator = metadataExampleGenerator;
        }

        public IEnumerable<IFileFormatInfo> AnalyseFileFormats(string targetPath, FileFormatScanMode scanMode)
        {
            return _fileFormatIdentifier.IdentifyFormats(targetPath, scanMode);
        }

        public void GenerateFileFormatInfoFiles(Archive archive)
        {
            DiasPackageWorkingDirectory diasPackageWorkingDirectory = archive.OutputDiasPackage.WorkingDirectory;
            try
            {
                var resultFileDirectoryPath = diasPackageWorkingDirectory.AdministrativeMetadata().ToString();
                string resultFileName;
                string resultFileFullName;

                if (archive is SiardArchive siardArchive)
                {
                    string siardFileFullName = siardArchive.SiardFile.FullName;

                    resultFileName = string.Format(OutputFileNames.FileFormatInfoFile, Path.GetFileNameWithoutExtension(siardFileFullName));
                    resultFileFullName = Path.Combine(resultFileDirectoryPath, resultFileName);

                    IEnumerable<KeyValuePair<string, IEnumerable<byte>>> lobsAsByte = _siardXmlTableReader.CreateLobByteArrays(siardFileFullName);
                    _fileFormatIdentifier.BroadCastStarted();
                    IEnumerable<IFileFormatInfo> formatAnalysedLobs = _fileFormatIdentifier.IdentifyFormats(lobsAsByte);
                    _fileFormatIdentifier.BroadCastFinished();
                    _fileFormatInfoGenerator.Generate(formatAnalysedLobs, siardFileFullName, resultFileFullName);
                }
                else if (archive is Noark5Archive noark5Archive)
                {
                    if (noark5Archive.SourceIsTarFile)
                    {
                        IEnumerable<IFileFormatInfo> analysedTarContents = _fileFormatIdentifier
                            .IdentifyFormats(noark5Archive.InputDiasPackage.TarFile.FullName, FileFormatScanMode.Archive)
                            .ToList();

                        string tarRootDirectoryName =
                            Path.GetFileNameWithoutExtension(noark5Archive.InputDiasPackage.TarFile.FullName);
                        string documentsDirectoryName = noark5Archive.GetDocumentsDirectoryName();

                        string tarFileRelativeDocumentsDirectoryPath = Path.Combine(tarRootDirectoryName!,
                            ArkadeConstants.DirectoryNameContent, documentsDirectoryName);

                        var fullDocumentsDirectoryTarPath =
                            $"{noark5Archive.InputDiasPackage.TarFile}#{tarFileRelativeDocumentsDirectoryPath}";

                        bool IsDocumentFile(IFileFormatInfo fileFormatInfo) =>
                            fileFormatInfo.FileName.StartsWith(fullDocumentsDirectoryTarPath);

                        IEnumerable<IFileFormatInfo> analysedDocumentFiles = analysedTarContents.Where(IsDocumentFile);

                        resultFileName = string.Format(OutputFileNames.FileFormatInfoFile, documentsDirectoryName);
                        resultFileFullName = Path.Combine(resultFileDirectoryPath, resultFileName);

                        _fileFormatInfoGenerator.Generate(analysedDocumentFiles, tarFileRelativeDocumentsDirectoryPath,
                            resultFileFullName);
                    }
                    else
                    {
                        DirectoryInfo documentsDirectory = noark5Archive.GetDocumentsDirectory();
                        resultFileName = string.Format(OutputFileNames.FileFormatInfoFile, documentsDirectory.Name);
                        resultFileFullName = Path.Combine(resultFileDirectoryPath, resultFileName);
                        IEnumerable<IFileFormatInfo> analysedFiles =
                            _fileFormatIdentifier.IdentifyFormats(documentsDirectory.FullName,
                                FileFormatScanMode.Directory);
                        _fileFormatInfoGenerator.Generate(analysedFiles, documentsDirectory.FullName,
                            resultFileFullName);
                    }
                }
            }
            catch (SiegfriedFileFormatIdentifierException siegfriedException)
            {
                Log.Error(siegfriedException.Message);
            }
            catch (Exception e)
            {
                Log.Debug(e.ToString());
                Log.Error("An unforeseen error related to document file format analysis has occured. As a result, document file format analysis was aborted. Please see /arkade-tmp/logs for details.");
            }
        }

        public void GenerateFileFormatInfoFiles(IEnumerable<IFileFormatInfo> fileFormatInfos, string relativePathRoot, string resultFileFullName, SupportedLanguage language)
        {
            LanguageManager.SetResourceLanguageForStandalonePronomAnalysis(language);

            _fileFormatInfoGenerator.Generate(fileFormatInfos, relativePathRoot, resultFileFullName);
        }

        public async Task<ArchiveFormatValidationReport> ValidateArchiveFormatAsync(
            FileSystemInfo item, ArchiveFormat format, string resultFileDirectoryPath, SupportedLanguage language)
        {
            // TODO: Resolve issues and re-enable PDF/A-validation
            if (format == ArchiveFormat.PdfA)
                throw new ArkadeException("Validation request with format PDF/A was rejected: 3rd party library issue");

            LanguageManager.SetResourceLanguageForArchiveFormatValidation(language);

            return await _archiveFormatValidator.ValidateAsync(item, format, resultFileDirectoryPath);
        }

        public void GenerateMetadataExampleFile(string outputFileName)
        {
            _metadataExampleGenerator.Generate(outputFileName);
        }

        public ArchiveType? DetectArchiveType(string archiveFileName)
        {
            return !Path.HasExtension(archiveFileName)
                ? _archiveTypeIdentifier.IdentifyTypeOfChosenArchiveDirectory(archiveFileName)
                : _archiveTypeIdentifier.IdentifyTypeOfChosenArchiveFile(archiveFileName);
        }
    }
}
