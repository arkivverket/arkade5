using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
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
            using var siardFileStream = new FileStream(siardFileName, FileMode.Open, FileAccess.Read);
            using var siardZipArchive = new ZipArchive(siardFileStream);

            Dictionary<string, List<int>> lobFolderPathsWithColumnIndexes =
                _siardArchiveReader.GetLobFolderPathsWithColumnIndexes(siardFileName);

            var formatAnalysedLobs = new List<IFileFormatInfo>();

            foreach ((string lobFolderPath, List<int> columnIndexes) in lobFolderPathsWithColumnIndexes)
            {
                foreach (int columnIndex in columnIndexes)
                {
                    XDocument xmlTableDoc = XDocument.Parse(GetXmlTableStringContent(siardFileName, lobFolderPath));

                    var xPathQuery = $"//*:c{columnIndex}";

                    IEnumerable<XElement> lobXmlElements = xmlTableDoc.XPath2SelectElements(xPathQuery);

                    if (!lobXmlElements.Any())
                    {
                        var message = $"Could not find any elements, {xPathQuery}, in {lobFolderPath}";
                        Log.Error(message);
                    }

                    foreach (XElement lobXmlElement in lobXmlElements)
                    {
                        formatAnalysedLobs.Add(GetFormatAnalysedLob(lobXmlElement, siardZipArchive, lobFolderPath));
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

        private string GetXmlTableStringContent(string archiveFileName, string lobFolderPath)
        {
            if (Path.GetExtension(archiveFileName) == ".siard")
                return GetXmlTableWhenArchiveIsFile(archiveFileName, lobFolderPath);
            else
                return GetXmlTableWhenArchiveIsDirectory(archiveFileName, lobFolderPath);
        }

        private string GetXmlTableWhenArchiveIsFile(string archiveFileName, string lobFolderPath)
        {
            string xmlTableFileName = lobFolderPath.Split('/')[1] + ".xml";
            using var siardFileStream = new FileStream(archiveFileName, FileMode.Open, FileAccess.Read);
            return _siardArchiveReader.GetNamedEntryFromSiardFileStream(siardFileStream, xmlTableFileName);
        }

        private string GetXmlTableWhenArchiveIsDirectory(string archiveFileName, string lobFolderPath)
        {
            string archiveContentFolderPath = Path.Combine(archiveFileName, ArkadeConstants.DirectoryNameContent);
            return File.ReadAllText(BuildLobTablePath(archiveContentFolderPath, lobFolderPath));
        }

        private string BuildLobTablePath(string archiveFolderPath, string lobFolderPath)
        {
            string[] lobFolderPathElements = lobFolderPath.Split('/');
            return Path.Combine(archiveFolderPath, lobFolderPathElements[0], lobFolderPathElements[1], lobFolderPathElements[1] + ".xml");
        }

        private IFileFormatInfo GetFormatAnalysedLob(XElement lobXmlElement, ZipArchive siardZipArchive, string lobFolderPath)
        {
            string lobFilePathFromTableXml = lobXmlElement.Attributes().FirstOrDefault(a => a.Name.LocalName.Equals("file"))?.Value;

            if (lobFilePathFromTableXml == null) //TODO: is this the correct way to check if the LOB is stored directly in the database?
            {
                //TODO: 
                /*var stream = new MemoryStream(Convert.FromBase64String(lobXmlElement.Value));
                return _fileFormatIdentifier.IdentifyFormat(stream, SiegfriedScanMode.Stream);*/

                Log.Error($"Attribute \"file\" not found on element \"{lobXmlElement.Name.LocalName}\"");
                return null;
            }

            IFileFormatInfo lobFormatInfo = _fileFormatIdentifier.IdentifyFormat
            (
                new KeyValuePair<string, Stream>
                (
                    GetLobFileRelativePath(lobFilePathFromTableXml, lobFolderPath),
                    _siardArchiveReader.GetNamedEntryStreamFromSiardZipArchive(siardZipArchive, lobFilePathFromTableXml)
                )
            );

            return lobFormatInfo;
        }
        
        private static string GetLobFileRelativePath(string lobFilePathFromTableXml, string lobFolderPath)
        {
            return "content/" + (lobFilePathFromTableXml.Contains(lobFolderPath)
                       ? lobFilePathFromTableXml
                       : lobFolderPath + lobFilePathFromTableXml);
        }
    }
}
