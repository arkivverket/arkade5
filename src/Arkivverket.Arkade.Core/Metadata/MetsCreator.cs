using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.ExternalModels.DiasMets;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Metadata
{
    public abstract class MetsCreator<T>
    {
        protected const string DateFormat = "yyyy-MM-dd";

        protected static XmlSerializerNamespaces SetupNamespaces()
        {
            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add("mets", "http://www.loc.gov/METS/");
            namespaces.Add("xlink", "http://www.w3.org/1999/xlink");
            namespaces.Add("xsi", "http://www.w3.org/2001/XMLSchema-instance");
            return namespaces;
        }

        protected static void CreateStructMap(dynamic mets)
        {
            mets.structMap = new[] { new structMapType { div = new divType() } };
        }

        protected static List<FileDescription> GetFileDescriptions(DirectoryInfo directory,
            DirectoryInfo pathRoot, ReadOnlyDictionary<string, DocumentFile> documentFiles,
            string[] filesToSkip = null)
        {
            var fileDescriptions = new List<FileDescription>();

            foreach (FileInfo file in GetFilesToDescribe(directory, filesToSkip))
                fileDescriptions.Add(GetFileDescription(file, pathRoot, documentFiles));

            return fileDescriptions;
        }

        protected static FileDescription GetFileDescription(FileInfo file, DirectoryInfo pathRoot,
            ReadOnlyDictionary<string, DocumentFile> documentFiles = null)
        {
            string name = pathRoot != null ? Path.GetRelativePath(pathRoot.FullName, file.FullName) : file.FullName;

            return new FileDescription
            {
                Name = name,
                Extension = file.Extension.Replace(".", string.Empty),
                Sha256Checksum = GetCheckSum(file, documentFiles, name),
                Size = file.Length,
                CreationTime = file.CreationTime
            };
        }

        protected IEnumerable<FileDescription> GetFileDescriptionsFromDocumentFiles(ReadOnlyDictionary<string, DocumentFile> documentFiles)
        {
            foreach ((var name, var documentFile) in documentFiles)
            {
                yield return GetFileDescriptionFromDocumentFile(name, documentFile);
            }
        }

        private FileDescription GetFileDescriptionFromDocumentFile(string name, DocumentFile documentFile)
        {
            return new FileDescription
            {
                Name = name,
                Extension = documentFile.Extension,
                Sha256Checksum = documentFile.CheckSum, // ?? GetSha256Checksum(documentFile.FileInfo), TODO ...
                Size = documentFile.Size,
                CreationTime = documentFile.CreationTime
            };
        }

        private static string GetCheckSum(FileInfo file, IReadOnlyDictionary<string, DocumentFile> documentFiles,
            string relativeFilePath)
        {
            return documentFiles != null &&
                   documentFiles.TryGetValue(relativeFilePath.Replace("\\", "/"), out DocumentFile documentFile) &&
                   documentFile.CheckSum != null
                ? documentFile.CheckSum
                : GetSha256Checksum(file);
        }

        private static IEnumerable<FileInfo> GetFilesToDescribe(DirectoryInfo directory, string[] filesToSkip)
        {
            IEnumerable<FileInfo> filesToDescribe = directory.EnumerateFiles(".", SearchOption.AllDirectories);
            
            return filesToSkip != null
                ? filesToDescribe.Where(f => !filesToSkip.Contains(f.Name))
                : filesToDescribe;
        }

        protected static void AutoIncrementFileIds(IEnumerable<FileDescription> fileDescriptions, int offset = 0)
        {
            int nextFileId = offset;
            foreach (FileDescription fileDescription in fileDescriptions)
                fileDescription.Id = nextFileId++;
        }

        private static string GetSha256Checksum(FileInfo file)
        {
            return new Sha256ChecksumGenerator().GenerateChecksum(file.FullName);
        }

        protected static bool HasEntity(MetadataEntityInformationUnit entityInformationUnit)
        {
            return !string.IsNullOrEmpty(entityInformationUnit.Entity);
        }

        protected static bool HasContactData(MetadataEntityInformationUnit entityInformationUnit)
        {
            return !string.IsNullOrEmpty(entityInformationUnit.ContactPerson) ||
                   !string.IsNullOrEmpty(entityInformationUnit.Address) ||
                   !string.IsNullOrEmpty(entityInformationUnit.Telephone) ||
                   !string.IsNullOrEmpty(entityInformationUnit.Email);
        }
    }
}
