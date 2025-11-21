using System;
using System.IO;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Base.Archives
{
    public abstract class Archive(DirectoryInfo processingDirectory)
    {
        public DirectoryInfo ProcessingDirectory { get; private init; } = processingDirectory;

        public bool SourceIsTarFile => InputDiasPackage?.TarFile != null;

        public required ArkadeDirectory Content { get; init; }

        public ArchiveContent WipContent { get; }
        public InputDiasPackage InputDiasPackage { get; protected init; }
        public OutputDiasPackage OutputDiasPackage { get; set; }
        public ArchiveType ArchiveType { get; protected init; } // TODO: Consider to liquidate

        public required IArchiveDetails Details { get; init; }
        public TestSession TestSession { get; set; }
        public abstract bool IsTestable(out string disqualifyingCause);
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