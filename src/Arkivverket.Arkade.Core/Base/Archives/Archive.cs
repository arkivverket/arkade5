using System;
using System.IO;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Base.Archives
{
    public abstract class Archive(DirectoryInfo processingDirectory, ArchiveContent content)
    {
        public DirectoryInfo ProcessingDirectory { get; private init; } = processingDirectory;

        public bool SourceIsTarFile => InputDiasPackage?.TarFile != null;

        public ArchiveContent Content { get; private init; } = content;
        public InputDiasPackage InputDiasPackage { get; protected init; }
        public OutputDiasPackage OutputDiasPackage { get; set; }
        public ArchiveType ArchiveType { get; protected init; } // TODO: Consider to liquidate

        public required IArchiveDetails Details { get; init; }
        public TestSession TestSession { get; set; }
        public abstract bool IsTestable(out string disqualifyingCause);
        
        protected static ArchiveContent SetupContent(InputDiasPackage inputDiasPackage)
        {
            return SetupContent(inputDiasPackage.WorkingDirectory.ContentWorkDirectory().DirectoryInfo());
        }
        
        protected static ArchiveContent SetupContent(DirectoryInfo archiveExtractionDirectory)
        {
            return new ArchiveContent(archiveExtractionDirectory.GetFileSystemInfos());
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