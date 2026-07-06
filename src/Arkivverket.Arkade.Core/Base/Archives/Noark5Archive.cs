using System.IO;
using System.Reflection;
using System.Text;
using Arkivverket.Arkade.Core.Base.Addml;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Util;
using ICSharpCode.SharpZipLib.Tar;
using Serilog;
using static Arkivverket.Arkade.Core.Util.ArkadeConstants;

namespace Arkivverket.Arkade.Core.Base.Archives;

public class Noark5Archive : AddmlBasedArchive
{
    private static readonly ILogger Log = Serilog.Log.ForContext(MethodBase.GetCurrentMethod().DeclaringType);
    public Noark5XmlUnits XmlUnits { get; }
    internal DocumentFiles DocumentFiles { get; init; }
    private DirectoryInfo DocumentsDirectory { get; set; }
    private string DocumentsDirectoryName { get; set; }

    public Noark5Archive(DirectoryArchiveContent content, DirectoryInfo processingDirectory, InputDiasPackage inputDiasPackage = null)
        : base(content, processingDirectory, inputDiasPackage)
    {
        DocumentFiles = SourceIsTarFile
            ? new DocumentFiles(InputDiasPackage.TarFile.FullName)
            : new DocumentFiles(GetDocumentsDirectory());

        if (AddmlXmlUnit != null)
            return;

        if (Content.GetFile(ArkivuttrekkXmlFileName) is not { } n5AddmlFile)
        {
            Log.Error("No archive description file found in archive.");
            return;
        }

        AddmlXmlUnit = SetupAddmlXmlUnit(n5AddmlFile);

        using (Stream schemaStream = AddmlXmlUnit.Schema.AsStream())
            AddmlInfo = AddmlUtil.ReadFromFile(AddmlXmlUnit.File.FullName, schemaStream);

        Details = new ArchiveDetails(AddmlInfo.Addml);

        XmlUnits = new Noark5XmlUnits(Content, Details as ArchiveDetails);
    }

    public ArchiveXmlFile GetArchiveXmlFile(string fileName)
    {
        return XmlUnits.Get(fileName)?.File;
    }

    // When re-packing from a tar, the content (including the document files) is streamed straight from
    // that tar and is not all present in Content, so the source tar's length is the accurate, O(1) size
    // source. Otherwise fall back to walking the content directory.
    public override long GetContentSize() =>
        SourceIsTarFile ? InputDiasPackage.TarFile.Length : base.GetContentSize();

    public DirectoryInfo GetDocumentsDirectory()
    {
        if (SourceIsTarFile)
            return null;

        if (DocumentsDirectory != null)
            return DocumentsDirectory;


        foreach (string documentDirectoryName in DocumentDirectoryNames)
        {
            if (Content.GetDirectory(documentDirectoryName) is not { } foundDocumentsDirectory)
                continue;

            // The directory search is case-insensitive on Windows; a supported name is an exact-case match only
            if (foundDocumentsDirectory.Name != documentDirectoryName)
                continue;

            DocumentsDirectory = foundDocumentsDirectory;
            return DocumentsDirectory;
        }

        return DefaultNamedDocumentsDirectory();
    }

    public string GetDocumentsDirectoryName()
    {
        if (DocumentsDirectoryName != null)
            return DocumentsDirectoryName;

        if (SourceIsTarFile)
        {
            var tarInputStream = new TarInputStream(File.OpenRead(InputDiasPackage.TarFile.FullName!), Encoding.UTF8);

            string archiveRootDirectoryName = Path.GetFileNameWithoutExtension(InputDiasPackage.TarFile.FullName);

            while (tarInputStream.GetNextEntry() is { Name: not null } entry)
            {
                if (!entry.IsDirectory && entry.IsNoark5DocumentsEntry(archiveRootDirectoryName))
                {
                    DocumentsDirectoryName = PathUtil.GetChild(DirectoryNameContent, entry.Name);
                    break;
                }
            }

            return DocumentsDirectoryName ?? DefaultNamedDocumentsDirectory().Name;
        }

        return GetDocumentsDirectory()?.Name;
    }

    private DirectoryInfo DefaultNamedDocumentsDirectory()
    {
        return new DirectoryInfo(DocumentDirectoryNames[0]);
    }

    public override bool IsTestable(out string disqualifyingCause)
    {
        if (AddmlXmlUnit == null)
        {
            disqualifyingCause = Noark5Messages.CouldNotFindValidSpecificationFile;
            return false;
        }

        disqualifyingCause = null;
        return true;
    }
}