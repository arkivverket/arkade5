using System;
using System.IO;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Base.Archives
{
    public abstract class Archive
    {
        public DirectoryInfo ProcessingDirectory { get; private init; }

        public bool SourceIsTarFile => InputDiasPackage?.TarFile != null;

        public ArkadeDirectory Content { get; protected init; }

        public ArchiveContent WipContent { get; }
        public InputDiasPackage InputDiasPackage { get; protected init; }
        public OutputDiasPackage OutputDiasPackage { get; set; }
        public ArchiveType ArchiveType { get; protected init; } // TODO: Consider to liquidate

        public IArchiveDetails Details { get; protected set; }
        public TestSession TestSession { get; set; }
        public abstract bool IsTestable(out string disqualifyingCause);

        protected Archive(DirectoryInfo processingDirectory)
        {
            ProcessingDirectory = processingDirectory;
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