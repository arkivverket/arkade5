using System;
using System.IO;
using System.Linq;

namespace Arkivverket.Arkade.Core.Base.Archives;

public abstract class Archive(IArchiveContent content, DirectoryInfo processingDirectory, InputDiasPackage inputDiasPackage = null)
{
    public readonly IArchiveContent Content = content;
    public DirectoryInfo ProcessingDirectory { get; } = processingDirectory;
    public bool SourceIsTarFile => InputDiasPackage?.TarFile != null;
    public InputDiasPackage InputDiasPackage { get; } = inputDiasPackage;
    public OutputDiasPackage OutputDiasPackage { get; set; }
    public ArchiveType ArchiveType => Enum.Parse<ArchiveType>(GetType().Name[..^"Archive".Length]);
    public IArchiveDetails Details { get; protected init; }
    public TestSession TestSession { get; set; }
    public abstract bool IsTestable(out string disqualifyingCause);

    /// <summary>
    /// Total size in bytes of the archive content that will be written into the package. Walks the content
    /// (and, for SIARD, its external LOBs) for an accurate figure. Overridden where an at-least-as-accurate
    /// but cheaper size source exists.
    /// </summary>
    public virtual long GetContentSize() => Content.GetFiles().Sum(contentFile => contentFile.File.Length);
}

public enum ArchiveType
{
    Noark3,
    Noark4,
    Noark5,
    SpecializedSystem,
    Siard,
}
