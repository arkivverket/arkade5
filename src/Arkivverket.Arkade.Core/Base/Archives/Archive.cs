using System;
using System.IO;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Base.Archives
{
    public abstract class Archive(DirectoryInfo processingDirectory, FileSystemInfo[] content)
    {
        public DirectoryInfo ProcessingDirectory { get; private init; } = processingDirectory;

        public bool SourceIsTarFile => InputDiasPackage?.TarFile != null;

        public ArkadeDirectory Content  { get; private init; }
        
        public FileSystemInfo[] ArchiveContent  { get; private init; } = content;
        
        public ArchiveContent WipContent { get; }
        public InputDiasPackage InputDiasPackage { get; protected init; }
        public OutputDiasPackage OutputDiasPackage { get; set; }
        public ArchiveType ArchiveType { get; protected init; } // TODO: Consider to liquidate

        public required IArchiveDetails Details { get; init; }
        public TestSession TestSession { get; set; }
        public abstract bool IsTestable(out string disqualifyingCause);
        
        protected static FileSystemInfo[] GetContent(InputDiasPackage inputDiasPackage)
        {
            return GetContent(inputDiasPackage.WorkingDirectory.ContentWorkDirectory().DirectoryInfo());
        }
        
        protected static FileSystemInfo[] GetContent(DirectoryInfo archiveExtractionDirectory)
        {
            return archiveExtractionDirectory.GetFileSystemInfos();
        }
    }

    [Flags]
    public enum ArchiveType
    {
        Noark3,
        Noark4,
        Noark5,
        SpecializedSystem,
        Siard,
    }
}