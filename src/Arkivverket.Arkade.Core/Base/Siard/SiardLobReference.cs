using System.IO;

namespace Arkivverket.Arkade.Core.Base.Siard
{
    public class SiardLobReference
    {
        public string SchemaFolder { get; init; }
        public SiardTable Table { get; init; }
        public SiardColumn Column { get; init; }
        public string FilePathInTableXml { get; set; }
        public string FilePathRelativeToContentFolder => GetPathFromContent();
        public string FilePathRelativeToLobFolder => GetPathFromLobFolder();
        public string LobFolderPath { get; init; }
        public bool IsExternal { get; init; }
        public int RowIndex { get; set; }

        public SiardLobReference(){}

        public SiardLobReference(SiardLobReference siardLobReference)
        {
            SchemaFolder = siardLobReference.SchemaFolder;
            Table = new SiardTable(siardLobReference.Table);
            Column = new SiardColumn(siardLobReference.Column);
            FilePathInTableXml = siardLobReference.FilePathInTableXml;
            LobFolderPath = siardLobReference.LobFolderPath;
            IsExternal = siardLobReference.IsExternal;
            RowIndex = siardLobReference.RowIndex;
        }

        private string GetPathFromContent()
        {
            string path;
            string columnFolderName = Column.FolderName ?? "";

            if (FilePathInTableXml.Contains(SchemaFolder) && FilePathInTableXml.Contains(Table.FolderName) && FilePathInTableXml.Contains(columnFolderName))
                path = FilePathInTableXml;
            else if (FilePathInTableXml.Contains(Table.FolderName) && FilePathInTableXml.Contains(columnFolderName))
                path = Path.Combine(SchemaFolder, FilePathInTableXml);
            else if (FilePathInTableXml.Contains(columnFolderName))
                path = Path.Combine(SchemaFolder, Table.FolderName, FilePathInTableXml);
            else if (columnFolderName.Contains(SchemaFolder) && columnFolderName.Contains(Table.FolderName))
                path = Path.Combine(columnFolderName, FilePathInTableXml);
            else if (columnFolderName.Contains(Table.FolderName))
                path = Path.Combine(SchemaFolder, columnFolderName, FilePathInTableXml);
            else
                path = Path.Combine(SchemaFolder, Table.FolderName, columnFolderName, FilePathInTableXml);

            return path.Replace('\\', '/');
        }

        private string GetPathFromLobFolder()
        {
            string[] splitPath = FilePathInTableXml.Split(Path.DirectorySeparatorChar, '/');
            if (splitPath.Length == 1) return FilePathInTableXml;

            string pathFromLobFolder = Path.Combine(splitPath[^2], splitPath[^1]);
            return pathFromLobFolder.Replace('\\', '/');
        }
    }

    public class SiardTable : SiardMetadataLevel
    {
        public string Name { get; init; }

        public SiardTable(){}

        public SiardTable(SiardTable siardTable)
        {
            Name = siardTable.Name;
            FolderName = siardTable.FolderName;
        }
    }

    public class SiardColumn : SiardMetadataLevel
    {
        public int Index { get; init; }
        public SiardColumn() { }

        public SiardColumn(SiardColumn siardColumn)
        {
            Index = siardColumn.Index;
            FolderName = siardColumn.FolderName;
        }
    }

    public abstract class SiardMetadataLevel
    {
        public string FolderName { get; init; }
    }
}
