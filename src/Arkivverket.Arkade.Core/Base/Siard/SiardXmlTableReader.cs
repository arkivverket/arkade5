using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Arkivverket.Arkade.Core.Util;
using Serilog;
using Wmhelp.XPath2;

namespace Arkivverket.Arkade.Core.Base.Siard
{
    public class SiardXmlTableReader : ISiardXmlTableReader
    {
        private static readonly ILogger Log = Serilog.Log.ForContext(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly ISiardArchiveReader _siardArchiveReader;

        public SiardXmlTableReader(ISiardArchiveReader siardArchiveReader)
        {
            _siardArchiveReader = siardArchiveReader;
        }

        internal IEnumerable<string> GetFullPathsToExternalLobs(string siardArchiveFullPath)
        {
            Dictionary<string, List<SiardLobReference>> lobFolderPathsWithColumnIndexes =
                _siardArchiveReader.GetLobFolderPathsWithColumnIndexes(siardArchiveFullPath);

            foreach (SiardLobReference siardLobReference in lobFolderPathsWithColumnIndexes.Values.SelectMany(l => l))
            {
                XDocument xmlTableDoc = XDocument.Parse(GetXmlTableStringContent(siardArchiveFullPath, siardLobReference));
                var xPathQuery = $"//*:c{siardLobReference.Column.Index}";

                List<XElement> lobXmlElements = xmlTableDoc.XPath2SelectElements(xPathQuery).ToList();

                foreach (XElement lobXmlElement in lobXmlElements)
                {
                    var lobReference = new SiardLobReference(siardLobReference)
                    {
                        FilePathInTableXml = lobXmlElement.Attributes().FirstOrDefault(a => a.Name.LocalName.Equals("file"))?.Value
                    };

                    if (!SiardLobIsExternal(lobReference)) continue;

                    yield return GetPathToExternalLob(siardArchiveFullPath, lobReference);
                }
            }
        }

        public IEnumerable<KeyValuePair<string, IEnumerable<byte>>> CreateLobByteArrays(string siardFileName)
        {
            Dictionary<string, List<SiardLobReference>> lobFolderPathsWithColumnIndexes =
                _siardArchiveReader.GetLobFolderPathsWithColumnIndexes(siardFileName);

            foreach ((string lobFolderPath, List<SiardLobReference> siardLobReferences) in lobFolderPathsWithColumnIndexes)
            {
                foreach (SiardLobReference siardLobReference in siardLobReferences)
                {
                    XDocument xmlTableDoc = XDocument.Parse(GetXmlTableStringContent(siardFileName, siardLobReference));
                    var xPathQuery = $"//*:c{siardLobReference.Column.Index}";
                    var xPathRowIndexQuery = $"//*:row[*:c{siardLobReference.Column.Index}=*]/*:c1";

                    List<XElement> lobXmlElements = xmlTableDoc.XPath2SelectElements(xPathQuery).ToList();
                    List<XElement> lobXmlElementsRowIndexElements = xmlTableDoc.XPath2SelectElements(xPathRowIndexQuery).ToList();

                    if (!lobXmlElements.Any())
                    {
                        var message = $"Could not find any elements, {xPathQuery}, in {lobFolderPath}";
                        Log.Error(message);
                    }

                    var indexCounter = 0;
                    foreach (XElement element in lobXmlElements)
                    {
                        if (int.TryParse(lobXmlElementsRowIndexElements[indexCounter].Value, out int rowIndex))
                            siardLobReference.RowIndex = rowIndex;

                        indexCounter++;

                        yield return CreateKeyValuePairForLob(element, siardFileName, new SiardLobReference(siardLobReference));
                    }
                }
            }
        }

        private string GetXmlTableStringContent(string archiveFileName, SiardLobReference siardLobReference)
        {
            return Path.GetExtension(archiveFileName) == ".siard"
                ? GetXmlTableWhenArchiveIsFile(archiveFileName, siardLobReference.Table.FolderName)
                : GetXmlTableWhenArchiveIsDirectory(archiveFileName, siardLobReference);
        }

        private string GetXmlTableWhenArchiveIsFile(string archiveFileName, string tableFolderName)
        {
            string xmlTableFileName = tableFolderName + ".xml";
            using var siardFileStream = new FileStream(archiveFileName, FileMode.Open, FileAccess.Read);
            return _siardArchiveReader.GetNamedEntryFromSiardFileStream(siardFileStream, xmlTableFileName);
        }

        private string GetXmlTableWhenArchiveIsDirectory(string archiveFileName, SiardLobReference siardLobReference)
        {
            string archiveContentFolderPath = Path.Combine(archiveFileName, ArkadeConstants.DirectoryNameContent);
            return File.ReadAllText(BuildLobTablePath(archiveContentFolderPath, siardLobReference.LobFolderPath));
        }

        private string BuildLobTablePath(string archiveFolderPath, string lobFolderPath)
        {
            string schemaFolderName = new Regex(@"/schema\d{1,4}").Match(lobFolderPath).Value;
            string tableFolderAndFileName = new Regex(@"/table\d{1,4}").Match(lobFolderPath).Value;
            return Path.Combine(archiveFolderPath, schemaFolderName, tableFolderAndFileName, tableFolderAndFileName + ".xml");
        }

        private KeyValuePair<string, IEnumerable<byte>> CreateKeyValuePairForLob(XElement lobXmlElement, string siardFileName, SiardLobReference siardLobReference)
        {
            siardLobReference.FilePathInTableXml = lobXmlElement.Attributes().FirstOrDefault(a => a.Name.LocalName.Equals("file"))?.Value;

            if (LobIsInlinedInXmlTable(siardLobReference.FilePathInTableXml))
            {
                return CreateKeyValuePairForInlinedLob(lobXmlElement, siardLobReference);
            }

            if (SiardLobIsExternal(siardLobReference))
            {
                return CreateKeyValuePairForExternalLobFile(siardFileName, siardLobReference);
            }

            return CreateKeyValuePairForInternalLobFile(siardFileName, siardLobReference);
        }

        private static bool LobIsInlinedInXmlTable(string siardLobFileReferenceFromTableXml)
        {
            return siardLobFileReferenceFromTableXml == null;
        }

        private static bool SiardLobIsExternal(SiardLobReference siardLobReference)
        {
            return !LobIsInlinedInXmlTable(siardLobReference.FilePathInTableXml) &&
                (siardLobReference.FilePathInTableXml?.StartsWith("..") == true || siardLobReference.IsExternal);
        }

        private KeyValuePair<string, IEnumerable<byte>> CreateKeyValuePairForInlinedLob(XElement lobXmlElement, SiardLobReference siardLobReference)
        {
            string siardLobLocation = siardLobReference.Table.FolderName + " - column" + siardLobReference.Column.Index +
                                      " - row" + siardLobReference.RowIndex;
            var bytes = new Span<byte>();

            if (!InlinedLobContentHasSupportedEncoding(lobXmlElement.Value, ref bytes))
                return new KeyValuePair<string, IEnumerable<byte>>(siardLobLocation, null);

            return new KeyValuePair<string, IEnumerable<byte>>(siardLobLocation, bytes.ToArray());
        }

        private static KeyValuePair<string, IEnumerable<byte>> CreateKeyValuePairForExternalLobFile(string siardFileName, SiardLobReference siardLobReference)
        {
            using FileStream stream = new FileInfo(GetPathToExternalLob(siardFileName, siardLobReference)).OpenRead();
            byte[] bytes = File.ReadAllBytes(GetPathToExternalLob(siardFileName, siardLobReference));
            return new KeyValuePair<string, IEnumerable<byte>>(siardLobReference.FilePathInTableXml, bytes);
        }

        private KeyValuePair<string, IEnumerable<byte>> CreateKeyValuePairForInternalLobFile(string siardFileName, SiardLobReference siardLobReference)
        {
            using var siardFileStream = new FileStream(siardFileName, FileMode.Open, FileAccess.Read);
            using var siardZipArchive = new ZipArchive(siardFileStream);
            using Stream stream = _siardArchiveReader.GetNamedEntryStreamFromSiardZipArchive(
                siardZipArchive, siardLobReference.FilePathRelativeToContentFolder);

            using var memoryStream = new MemoryStream();
            stream.CopyTo(memoryStream);

            byte[] bytes = memoryStream.ToArray();

            return new KeyValuePair<string, IEnumerable<byte>>
            (
                Path.Combine("content", siardLobReference.FilePathRelativeToContentFolder),
                bytes
            );
        }

        private bool InlinedLobContentHasSupportedEncoding(string value, ref Span<byte> bytes)
        {
            if (StringIsHex(value))
            {
                bytes = Convert.FromHexString(value);
                return true;
            }

            return Convert.TryFromBase64String(value, bytes, out _);
        }

        private static bool StringIsHex(string input)
        {
            return Regex.IsMatch(input, @"\A\b[0-9a-fA-F]+\b\Z");
        }

        private static string GetPathToExternalLob(string siardFileName, SiardLobReference siardLobReference)
        {
            string siardFileDirectoryPath = siardFileName.Replace(Path.GetFileName(siardFileName), string.Empty);

            string relativePathToLobFile = siardLobReference.FilePathInTableXml?.StartsWith("..") == true
                ? siardLobReference.FilePathInTableXml.Remove(0, 3)
                : Path.Combine(siardLobReference.LobFolderPath.TrimStart('.', '\\', '/'),
                    siardLobReference.FilePathRelativeToLobFolder);

            string pathToExternalLob = Path.Combine(siardFileDirectoryPath, relativePathToLobFile);
            return pathToExternalLob.Replace('\\', '/');
        }
    }
}
