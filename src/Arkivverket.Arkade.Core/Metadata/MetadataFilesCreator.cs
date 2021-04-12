using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Base.Siard;
using Arkivverket.Arkade.Core.Report;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Util;
using Arkivverket.Arkade.Core.Util.FileFormatIdentification;
using Serilog;

namespace Arkivverket.Arkade.Core.Metadata
{
    public class MetadataFilesCreator
    {
        private static readonly ILogger Log = Serilog.Log.ForContext(MethodBase.GetCurrentMethod()?.DeclaringType);

        private readonly DiasMetsCreator _diasMetsCreator;
        private readonly DiasPremisCreator _diasPremisCreator;
        private readonly LogCreator _logCreator;
        private readonly EacCpfCreator _eacCpfCreator;
        private readonly EadCreator _eadCreator;
        private readonly ISiardXmlTableReader _siardXmlTableReader;
        private readonly ISiardArchiveReader _siardArchiveReader;

        public MetadataFilesCreator(DiasMetsCreator diasMetsCreator, DiasPremisCreator diasPremisCreator,
            EadCreator eadCreator, EacCpfCreator eacCpfCreator, LogCreator logCreator, ISiardXmlTableReader siardXmlTableReader,
            ISiardArchiveReader siardArchiveReader)
        {
            _diasMetsCreator = diasMetsCreator;
            _diasPremisCreator = diasPremisCreator;
            _logCreator = logCreator;
            _eadCreator = eadCreator;
            _eacCpfCreator = eacCpfCreator;
            _siardXmlTableReader = siardXmlTableReader;
            _siardArchiveReader = siardArchiveReader;
        }

        public void Create(Archive archive, ArchiveMetadata metadata, bool generateFileFormatInfo)
        {
            _diasPremisCreator.CreateAndSaveFile(archive, metadata);
            _logCreator.CreateAndSaveFile(archive, metadata);
            // EAD is not included in v1.0
            _eadCreator.CreateAndSaveFile(archive, metadata);
            // EAC-CPF is not included in v1.0
            _eacCpfCreator.CreateAndSaveFile(archive, metadata);

            CopyXsdFiles(archive.WorkingDirectory);

            if (archive.ArchiveType == ArchiveType.Siard)
                ExtractSiardMetadataFiles(archive);

            if (generateFileFormatInfo)
            {
                try
                {
                    string resultFileDirectoryPath = archive.WorkingDirectory.AdministrativeMetadata().DirectoryInfo().FullName;
                    string resultFileName;
                    string resultFileFullName;
                    
                    if (archive.ArchiveType == ArchiveType.Siard)
                    {
                        string headerDirectoryPath = Path.Combine(
                            archive.WorkingDirectory.Content().DirectoryInfo().FullName, 
                            ArkadeConstants.SiardHeaderDirectoryName);

                        string archivePath = archive.WorkingDirectory.HasExternalContentDirectory() &&
                                             Directory.Exists(headerDirectoryPath)
                            ? archive.WorkingDirectory.Content().DirectoryInfo().FullName
                            : archive.WorkingDirectory.Content().DirectoryInfo().GetFiles("*.siard")[0].FullName;

                        string archiveFileName = Path.GetFileName(archivePath);

                        resultFileName = string.Format(OutputFileNames.FileFormatInfoFile, archiveFileName);
                        resultFileFullName = Path.Combine(resultFileDirectoryPath, resultFileName);

                        List<IFileFormatInfo> formatAnalysedLobs = _siardXmlTableReader.GetFormatAnalysedLobs(archivePath);
                        FileFormatInfoGenerator.Generate(formatAnalysedLobs, archivePath, resultFileFullName);
                    }
                    else
                    {
                        DirectoryInfo documentsDirectory = archive.GetDocumentsDirectory();
                        resultFileName = string.Format(OutputFileNames.FileFormatInfoFile, documentsDirectory.Name);
                        resultFileFullName = Path.Combine(resultFileDirectoryPath, resultFileName);
                        FileFormatInfoGenerator.Generate(archive.GetDocumentsDirectory(), resultFileFullName);
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

            // Generate mets-file last for it to describe all other package content
            _diasMetsCreator.CreateAndSaveFile(archive, metadata);
        }

        private static void CopyXsdFiles(WorkingDirectory workingDirectory)
        {
            CopyXsdFile(
                ArkadeConstants.DiasMetsXsdResource,
                ArkadeConstants.DiasMetsXsdFileName,
                workingDirectory.Root()
            );

            CopyXsdFile(
                ArkadeConstants.DiasPremisXsdResource,
                ArkadeConstants.DiasPremisXsdFileName,
                workingDirectory.AdministrativeMetadata()
            );

            CopyXsdFile(
                ArkadeConstants.AddmlXsdResource,
                ArkadeConstants.AddmlXsdFileName,
                workingDirectory.AdministrativeMetadata()
            );
        }

        private static void CopyXsdFile(string xsdResource, string xsdFileName, ArkadeDirectory arkadeDirectory)
        {
            using (Stream xsdResourceStream = ResourceUtil.GetResourceAsStream(xsdResource))
            {
                string targetXsdFileName = arkadeDirectory.WithFile(xsdFileName).FullName;

                using (FileStream targetXsdFileStream = File.Create(targetXsdFileName))
                {
                    xsdResourceStream.CopyTo(targetXsdFileStream);
                }
            }
        }

        private void ExtractSiardMetadataFiles(Archive archive)
        {
            var administrativeMetadataPath = archive.WorkingDirectory.AdministrativeMetadata().ToString();
            string sourceDirectory = Path.Combine(
                archive.WorkingDirectory.Content().DirectoryInfo().FullName,
                ArkadeConstants.SiardHeaderDirectoryName
            );
            if (archive.WorkingDirectory.HasExternalContentDirectory() && Directory.Exists(sourceDirectory))
            {
                CopySiardMetadataFile(
                    ArkadeConstants.SiardMetadataXmlFileName, sourceDirectory, administrativeMetadataPath
                );
                CopySiardMetadataFile(
                    ArkadeConstants.SiardMetadataXsdFileName, sourceDirectory, administrativeMetadataPath
                );
            }
            else
            {
                string archiveFilePath =
                    archive.WorkingDirectory.Content().DirectoryInfo().GetFiles("*.siard")[0].FullName;
                ExtractSiardMetadataFile(ArkadeConstants.SiardMetadataXmlFileName, administrativeMetadataPath,
                    archiveFilePath);
                ExtractSiardMetadataFile(ArkadeConstants.SiardMetadataXsdFileName, administrativeMetadataPath,
                    archiveFilePath);
            }
        }

        private static void CopySiardMetadataFile(string fileName, string sourceDirectory, string targetDirectory)
        {
            string sourceFileName = Path.Combine(sourceDirectory, fileName);
            string targetFileName = Path.Combine(targetDirectory, fileName);
            File.Copy(sourceFileName, targetFileName);
        }

        private void ExtractSiardMetadataFile(string fileName, string targetDirectory, string archiveFilePath)
        {
            string targetFileName = Path.Combine(targetDirectory, fileName);
            using var siardFileStream = new FileStream(archiveFilePath, FileMode.Open, FileAccess.Read);
            string fileContent = _siardArchiveReader.GetNamedEntryFromSiardFileStream(siardFileStream, fileName);
            using FileStream targetFileStream = File.Create(targetFileName);
            using var streamWriter = new StreamWriter(targetFileStream, Encodings.UTF8);
            streamWriter.Write(fileContent);
        }
    }
}