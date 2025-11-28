using System;
using System.IO;

namespace Arkivverket.Arkade.Core.Base.Archives;

public abstract class Archive(IArchiveContent content, DirectoryInfo processingDirectory, InputDiasPackage inputDiasPackage = null)
{
    public readonly IArchiveContent Content = content;
    public DirectoryInfo ProcessingDirectory { get; } = processingDirectory;
    public bool SourceIsTarFile => InputDiasPackage?.TarFile != null;
    public InputDiasPackage InputDiasPackage { get; } = inputDiasPackage; // Why is private init different here than for Content?
    public OutputDiasPackage OutputDiasPackage { get; set; }
    public ArchiveType ArchiveType { get; protected init; } // TODO: Consider to liquidate
    public IArchiveDetails Details { get; protected init; }
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
