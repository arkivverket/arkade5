using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Arkivverket.Arkade.Core.ExternalModels.Metadata;
using Arkivverket.Arkade.Core.ExternalModels.Siard1Metadata;
using Arkivverket.Arkade.Core.Util;
using Serilog;
using siard2ColumnType = Arkivverket.Arkade.Core.ExternalModels.Metadata.columnType;
using siard1ColumnType = Arkivverket.Arkade.Core.ExternalModels.Siard1Metadata.columnType;
using siard2Archive = Arkivverket.Arkade.Core.ExternalModels.Metadata.siardArchive;
using siard1Archive = Arkivverket.Arkade.Core.ExternalModels.Siard1Metadata.siardArchive;

namespace Arkivverket.Arkade.Core.Base.Siard
{
    public class SiardArchiveReader : ISiardArchiveReader
    {
        public Dictionary<string, List<SiardLobReference>> GetLobFolderPathsWithColumnIndexes(string siardArchivePath)
        {
            const int zeroIndexOffset = 1;

            var lobTablePathsWithColumnIndexes = new Dictionary<string, List<SiardLobReference>>();

            var siardArchive = DeserializeMetadataXmlFromArchiveFile(siardArchivePath);

            if (siardArchive.GetType().FullName.Contains("ExternalModels.Metadata"))
                return GetLobFolderPathsFromSiard2Archive(siardArchive as siard2Archive, lobTablePathsWithColumnIndexes, zeroIndexOffset);

            if (siardArchive.GetType().FullName.Contains("ExternalModels.Siard1Metadata"))
                return GetLobFolderPathsFromSiard1Archive(siardArchive as siard1Archive, lobTablePathsWithColumnIndexes, zeroIndexOffset);

            return null;
        }

        public object DeserializeMetadataXmlFromArchiveFile(string siardArchivePath)
        {
            using var siardFileStream = new FileStream(siardArchivePath, FileMode.Open, FileAccess.Read);
            string metadataXmlStringContent = GetNamedEntryFromSiardFileStream(
                siardFileStream, ArkadeConstants.SiardMetadataXmlFileName
            );

            object siardArchive;

            try
            {
                siardArchive = SerializeUtil.DeserializeFromString<siard2Archive>(metadataXmlStringContent);
            }
            catch (Exception e)
            {
                try
                {
                    siardArchive = SerializeUtil.DeserializeFromString<siard1Archive>(metadataXmlStringContent);
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

        private Dictionary<string, List<SiardLobReference>> GetLobFolderPathsFromSiard2Archive(siard2Archive siardArchive,
            Dictionary<string, List<SiardLobReference>> lobFolderPathsWithColumnIndexes, int zeroIndexOffset)
        {
            bool hasExternalLobs = siardArchive.lobFolder != null && siardArchive.lobFolder.StartsWith("..");
            foreach (var schema in siardArchive.schemas)
            {
                foreach (var table in schema.tables)
                {
                    for (var columnIndex = 0; columnIndex < table.columns.Length; columnIndex++)
                    {
                        var column = table.columns[columnIndex];
                        for (var i = 0; i < column.Items.Length; i++)
                        {
                            if (Siard2ColumnDoesNotHaveLobContent(column, i))
                                continue;

                            var siardTable = new SiardTable {Name = table.name, FolderName = table.folder,};
                            var siardLobReference = new SiardLobReference
                            {
                                SchemaFolder = schema.folder,
                                Table = siardTable,
                                Column = new SiardColumn
                                {
                                    FolderName = column.lobFolder,
                                    Index = columnIndex + zeroIndexOffset,
                                },
                                IsExternal = hasExternalLobs,
                                LobFolderPath = Path.Combine(hasExternalLobs ? siardArchive.lobFolder : string.Empty,
                                    GetLobFolderPath(schema.folder, siardTable, column.lobFolder, siardArchive.producerApplication)),
                            };

                            if (!lobFolderPathsWithColumnIndexes.TryAdd(siardLobReference.LobFolderPath,
                                new List<SiardLobReference> {siardLobReference}))
                                lobFolderPathsWithColumnIndexes[siardLobReference.LobFolderPath].Add(siardLobReference);
                        }
                    }
                }
            }

            return lobFolderPathsWithColumnIndexes;
        }

        private static string GetLobFolderPath(string schemaFolder, SiardTable table, string columnLobFolder,
            string producerApplication)
        {
            if (producerApplication.ToLower().Contains("siardgui"))
                return table.Name;
            if (columnLobFolder.Contains(schemaFolder) && columnLobFolder.Contains(table.FolderName))
                return columnLobFolder;

            if (columnLobFolder.Contains(table.FolderName))
                return Path.Combine(schemaFolder, columnLobFolder);

            return Path.Combine(schemaFolder, table.FolderName, columnLobFolder);
        }

        private static bool Siard2ColumnDoesNotHaveLobContent(siard2ColumnType column, int i)
        {
            if (column.ItemsElementName[i] != ItemsChoiceType1.type)
                return true;

            if (!column.Items[i].Contains("LOB", StringComparison.OrdinalIgnoreCase) &&
                !column.Items[i].Contains("LARGE OBJECT", StringComparison.OrdinalIgnoreCase))
                return true;

            return false;
        }

        private Dictionary<string, List<SiardLobReference>> GetLobFolderPathsFromSiard1Archive(siard1Archive siardArchive,
            Dictionary<string, List<SiardLobReference>> lobFolderPathsWithColumnIndexes, int zeroIndexOffset)
        {
            foreach (var schema in siardArchive.schemas)
            {
                foreach (var table in schema.tables)
                {
                    string pathFromContentFolderToLobFolder = schema.folder + "/" + table.folder;

                    for (var columnIndex = 0; columnIndex < table.columns.Length; columnIndex++)
                    {
                        var column = table.columns[columnIndex];

                        if (Siard1ColumnDoesNotHaveLobContent(column))
                            continue;
                        
                        pathFromContentFolderToLobFolder = PathUtil.Merge(column.folder, pathFromContentFolderToLobFolder);
                    
                        var siardTable = new SiardTable {Name = table.name, FolderName = table.folder}; 
                        var siardLobReference = new SiardLobReference
                        {
                            SchemaFolder = schema.folder,
                            Table = siardTable,
                            Column = new SiardColumn
                            {
                                FolderName = column.folder,
                                Index = columnIndex + zeroIndexOffset,
                            },
                            LobFolderPath = GetLobFolderPath(schema.folder, siardTable, column.folder, siardArchive.producerApplication),
                        };

                        if (!lobFolderPathsWithColumnIndexes.TryAdd(siardLobReference.LobFolderPath,
                            new List<SiardLobReference> { siardLobReference }))
                            lobFolderPathsWithColumnIndexes[siardLobReference.LobFolderPath].Add(siardLobReference);
                    }
                }
            }

            return lobFolderPathsWithColumnIndexes;
        }

        private static bool Siard1ColumnDoesNotHaveLobContent(siard1ColumnType column)
        {
            return !column.typeOriginal.Contains("LOB", StringComparison.OrdinalIgnoreCase) &&
                   !column.type.Contains("LARGE OBJECT", StringComparison.OrdinalIgnoreCase);
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
