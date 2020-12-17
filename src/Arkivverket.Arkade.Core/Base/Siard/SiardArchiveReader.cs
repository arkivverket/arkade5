using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Arkivverket.Arkade.Core.ExternalModels.Metadata;
using Arkivverket.Arkade.Core.ExternalModels.Siard1Metadata;
using Arkivverket.Arkade.Core.Util;
using Serilog;
using siardArchive2_1 = Arkivverket.Arkade.Core.ExternalModels.Metadata.siardArchive;
using siardArchive1_0 = Arkivverket.Arkade.Core.ExternalModels.Siard1Metadata.siardArchive;

namespace Arkivverket.Arkade.Core.Base.Siard
{
    public class SiardArchiveReader : ISiardArchiveReader
    {
        public Dictionary<string, List<int>> GetLobFolderPathsWithColumnIndexes(string siardArchivePath)
        {
            const int zeroIndexOffset = 1;

            var lobTablePathsWithColumnIndexes = new Dictionary<string, List<int>>();

            var siardArchive = DeserializeMetadataXmlFromArchiveFile(siardArchivePath);

            if (siardArchive.GetType().FullName.Contains("ExternalModels.Metadata"))
                return GetLobFolderPathsFromSiard2_1Archive(siardArchive as siardArchive2_1, lobTablePathsWithColumnIndexes, zeroIndexOffset);

            if (siardArchive.GetType().FullName.Contains("ExternalModels.Siard1Metadata"))
                return GetLobFolderPathsFromSiard1_0Archive(siardArchive as siardArchive1_0, lobTablePathsWithColumnIndexes, zeroIndexOffset);

            return null;
        }

        private Dictionary<string, List<int>> GetLobFolderPathsFromSiard2_1Archive(
            siardArchive2_1 siardArchive,
            Dictionary<string, List<int>> lobFolderPathsWithColumnIndexes,
            int zeroIndexOffset)
        {
            foreach (var schema in siardArchive.schemas)
            {
                foreach (var table in schema.tables)
                {
                    string pathFromContentFolderToLobFolder = schema.folder + "/" + table.folder;

                    for (var columnIndex = 0; columnIndex < table.columns.Length; columnIndex++)
                    {
                        var column = table.columns[columnIndex];
                        for (var i = 0; i < column.Items.Length; i++)
                        {
                            if (column.ItemsElementName[i] != ItemsChoiceType1.type)
                                continue;

                            if (!column.Items[i].Contains("LOB", StringComparison.OrdinalIgnoreCase) &&
                                !column.Items[i].Contains("LARGE OBJECT", StringComparison.OrdinalIgnoreCase))
                                continue;

                            if (!TryGetLobFolderFromColumnElement(column.lobFolder, ref pathFromContentFolderToLobFolder) &&
                                siardArchive.lobFolder != null)
                                pathFromContentFolderToLobFolder = siardArchive.lobFolder;

                            try
                            {
                                if (!lobFolderPathsWithColumnIndexes.TryAdd(pathFromContentFolderToLobFolder,
                                    new List<int> {columnIndex + zeroIndexOffset}))
                                    lobFolderPathsWithColumnIndexes[pathFromContentFolderToLobFolder]
                                        .Add(columnIndex + zeroIndexOffset);
                            }
                            catch (Exception e)
                            {
                                string message = $"Could not find a valid reference to {schema.name}/{table.name}/c{columnIndex} - {column.name}";
                                Log.Error(message +"\n" + e.Message);
                            }
                        }
                    }
                }
            }

            return lobFolderPathsWithColumnIndexes;
        }

        private Dictionary<string, List<int>> GetLobFolderPathsFromSiard1_0Archive(
            siardArchive1_0 siardArchive,
            Dictionary<string, List<int>> lobFolderPathsWithColumnIndexes,
            int zeroIndexOffset)
        {
            foreach (var schema in siardArchive.schemas)
            {
                foreach (var table in schema.tables)
                {
                    string pathFromContentFolderToLobFolder = schema.folder + "/" + table.folder;

                    for (var columnIndex = 0; columnIndex < table.columns.Length; columnIndex++)
                    {
                        var column = table.columns[columnIndex];

                        if (!column.typeOriginal.Contains("LOB", StringComparison.OrdinalIgnoreCase) &&
                            !column.type.Contains("LARGE OBJECT", StringComparison.OrdinalIgnoreCase))
                            continue;

                        TryGetLobFolderFromColumnElement(column.folder, ref pathFromContentFolderToLobFolder);

                        try
                        {
                            if (!lobFolderPathsWithColumnIndexes.TryAdd(pathFromContentFolderToLobFolder,
                                new List<int> {columnIndex + zeroIndexOffset}))
                                lobFolderPathsWithColumnIndexes[pathFromContentFolderToLobFolder]
                                    .Add(columnIndex + zeroIndexOffset);
                        }
                        catch (Exception e)
                        {
                            var message = $"Could not find a valid reference to {schema.name}/{table.name}/c{columnIndex} - {column.name}";
                            Log.Error(message + "\n" + e.Message);
                        }
                    }
                }
            }

            return lobFolderPathsWithColumnIndexes;
        }

        private object DeserializeMetadataXmlFromArchiveFile(string siardArchivePath)
        {
            using var siardFileStream = new FileStream(siardArchivePath, FileMode.Open, FileAccess.Read);
            string metadataXmlStringContent = GetNamedEntryFromSiardFileStream(
                siardFileStream, ArkadeConstants.SiardMetadataXmlFileName
            );

            object siardArchive;

            try
            {
                siardArchive = SerializeUtil.DeserializeFromString<siardArchive2_1>(metadataXmlStringContent);
            }
            catch (Exception e)
            {
                try
                {
                    siardArchive = SerializeUtil.DeserializeFromString<siardArchive1_0>(metadataXmlStringContent);
                }
                catch (Exception e2)
                {
                    var message = $"Deserialisation of {ArkadeConstants.SiardMetadataXmlFileName} failed: {e.Message}\n{e2.Message}";
                    Log.Error(message);
                    return null;
                }
            }

            return siardArchive;
        }

        private bool TryGetLobFolderFromColumnElement(string lobFolder, ref string pathFromContentFolderToLobFolder)
        {
            if (string.IsNullOrWhiteSpace(lobFolder))
                return false;

            if (lobFolder.Contains(pathFromContentFolderToLobFolder))
            {
                pathFromContentFolderToLobFolder = lobFolder;
                return true;
            }

            pathFromContentFolderToLobFolder += "/" + lobFolder;
            return true;
        }

        public string GetNamedEntryFromSiardFileStream(Stream siardFileStream, string namedEntry)
        {
            using var zipArchive = new ZipArchive(siardFileStream);

            using Stream entryStream = zipArchive.Entries.FirstOrDefault(e => e.FullName.Contains(namedEntry))?.Open();

            if (entryStream == null)
                return null;

            using var streamReader = new StreamReader(entryStream, Encodings.ISO_8859_1);

            return streamReader.ReadToEnd();
        }

        public Stream GetNamedEntryStreamFromSiardZipArchive(ZipArchive siardZipArchive, string namedEntry)
        {
            return siardZipArchive.Entries.FirstOrDefault(e => e.FullName.Contains(namedEntry))?.Open();
        }
    }
}
