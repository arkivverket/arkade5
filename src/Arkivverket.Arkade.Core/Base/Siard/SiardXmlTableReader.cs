using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Arkivverket.Arkade.Core.Util;
using Arkivverket.Arkade.Core.Util.FileFormatIdentification;
using Serilog;
using Wmhelp.XPath2;

namespace Arkivverket.Arkade.Core.Base.Siard
{
    public class SiardXmlTableReader : ISiardXmlTableReader
    {
        private static readonly ILogger Log = Serilog.Log.ForContext(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly ISiardArchiveReader _siardArchiveReader;
        private readonly IFileFormatIdentifier _fileFormatIdentifier;

        public SiardXmlTableReader(ISiardArchiveReader siardArchiveReader, IFileFormatIdentifier fileFormatIdentifier)
        {
            _siardArchiveReader = siardArchiveReader;
            _fileFormatIdentifier = fileFormatIdentifier;
        }

        public List<IFileFormatInfo> GetFormatAnalysedLobs(string siardFileName)
        {
            Dictionary<string, List<SiardLobReference>> lobFolderPathsWithColumnIndexes =
                _siardArchiveReader.GetLobFolderPathsWithColumnIndexes(siardFileName);

            var formatAnalysedLobs = new List<IFileFormatInfo>();

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

                    for (var i = 0; i < lobXmlElements.Count(); i++)
                    {
                        if (int.TryParse(lobXmlElementsRowIndexElements[i].Value, out int rowIndex))
                            siardLobReference.RowIndex = rowIndex;

                        formatAnalysedLobs.Add(GetFormatAnalysedLob(lobXmlElements[i], siardFileName, siardLobReference));
                    }

                    if (formatAnalysedLobs.Any(f => f == null))
                    {
                        Log.Error(formatAnalysedLobs.Count(f => f == null) +
                                  " BLOBs/CLOBs were not analysed. Look for previous error messages in logfile for details.");
                    }
                }
            }

            return formatAnalysedLobs;
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

        private IFileFormatInfo GetFormatAnalysedLob(XElement lobXmlElement, string siardFileName, SiardLobReference siardLobReference)
        {
            siardLobReference.FilePathInTableXml = lobXmlElement.Attributes().FirstOrDefault(a => a.Name.LocalName.Equals("file"))?.Value;

            if (LobIsInlinedInXmlTable(siardLobReference.FilePathInTableXml))
            {
                return RunFormatAnalysisOnInlinedLob(lobXmlElement, siardLobReference);
            }

            if (SiardLobIsExternal(siardLobReference))
            {
                return RunFormatAnalysisOnExternalLobFile(siardFileName, siardLobReference);
            }

            return RunFormatAnalysisOnInternalLobFile(siardFileName, siardLobReference);
        }

        private static bool LobIsInlinedInXmlTable(string siardLobFileReferenceFromTableXml)
        {
            return siardLobFileReferenceFromTableXml == null;
        }

        private static bool SiardLobIsExternal(SiardLobReference siardLobReference)
        {
            return siardLobReference.FilePathInTableXml.StartsWith("..") || siardLobReference.IsExternal;
        }

        private IFileFormatInfo RunFormatAnalysisOnInlinedLob(XElement lobXmlElement, SiardLobReference siardLobReference)
        {
            string siardLobLocation = siardLobReference.Table.FolderName + " - column" + siardLobReference.Column.Index +
                                      " - row" + siardLobReference.RowIndex;
            try
            {
                var bytes = new Span<byte>();
                if (!InlinedLobContentHasSupportedEncoding(lobXmlElement.Value, ref bytes))
                {
                    return new SiegfriedFileInfo(siardLobLocation,
                        Resources.SiardMessages.InlinedLobContentHasUnsupportedEncoding, "N/A", "N/A", "N/A", "N/A");
                }

                using var stream = new MemoryStream(bytes.ToArray());
                return _fileFormatIdentifier.IdentifyFormat
                (
                    new KeyValuePair<string, Stream>(siardLobLocation, stream)
                );
            }
            catch (Exception e)
            {
                HandleLobAnalysisException(e, siardLobLocation);
                return null;
            }
        }

        private IFileFormatInfo RunFormatAnalysisOnExternalLobFile(string siardFileName, SiardLobReference siardLobReference)
        {
            try
            {
                using FileStream stream = new FileInfo(
                    GetPathToExternalLob(siardFileName, siardLobReference)).OpenRead();

                return _fileFormatIdentifier.IdentifyFormat
                (
                    new KeyValuePair<string, Stream>(siardLobReference.FilePathInTableXml, stream)
                );
            }
            catch (Exception e)
            {
                HandleLobAnalysisException(e, siardLobReference.FilePathInTableXml);
                return null;
            }
        }

        private IFileFormatInfo RunFormatAnalysisOnInternalLobFile(string siardFileName, SiardLobReference siardLobReference)
        {
            try
            {
                using var siardFileStream = new FileStream(siardFileName, FileMode.Open, FileAccess.Read);
                using var siardZipArchive = new ZipArchive(siardFileStream);
                using Stream stream = _siardArchiveReader.GetNamedEntryStreamFromSiardZipArchive(
                    siardZipArchive, siardLobReference.FilePathRelativeToContentFolder);

                return _fileFormatIdentifier.IdentifyFormat
                (
                    new KeyValuePair<string, Stream>
                    (
                        Path.Combine("content", siardLobReference.FilePathRelativeToContentFolder),
                        stream
                    )
                );
            }
            catch (Exception e)
            {
                HandleLobAnalysisException(e, siardLobReference.FilePathRelativeToContentFolder);
                return null;
            }
        }

        private static void HandleLobAnalysisException(Exception e, string siardLobIdentifier)
        {
            Log.Debug(e.ToString());
            Log.Error($"Was not able to analyse {siardLobIdentifier} - please see logfile for details.");
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

            string relativePathToLobFile = siardLobReference.FilePathInTableXml.StartsWith("..")
                ? siardLobReference.FilePathInTableXml.Remove(0, 3)
                : Path.Combine(siardLobReference.LobFolderPath.TrimStart('.', '\\', '/'),
                    siardLobReference.FilePathRelativeToLobFolder);

            string pathToExternalLob = Path.Combine(siardFileDirectoryPath, relativePathToLobFile);
            return pathToExternalLob;
        }
    }
}
