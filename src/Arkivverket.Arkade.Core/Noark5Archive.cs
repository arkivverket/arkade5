using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Resources;
using Serilog;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Arkivverket.Arkade.Core.Util;
using ICSharpCode.SharpZipLib.Tar;
using static Arkivverket.Arkade.Core.Util.ArkadeConstants;

namespace Arkivverket.Arkade.Core;

public class Noark5Archive : Archive
{
    private static readonly ILogger Log = Serilog.Log.ForContext(MethodBase.GetCurrentMethod().DeclaringType);
    public List<ArchiveXmlUnit> XmlUnits { get; private set; }
    internal DocumentFiles DocumentFiles { get; init; }
    private DirectoryInfo DocumentsDirectory { get; set; }
    private string DocumentsDirectoryName { get; set; }

    public Noark5Archive(DirectoryInfo archiveExtractionDirectory, DirectoryInfo processingDirectory) : base(processingDirectory)
    {
        ArchiveType = ArchiveType.Noark5; // TODO: Get rid of this ...
        
        Content = new ArkadeDirectory(archiveExtractionDirectory);
        
        SetupConstructorCommonThingsAndOfCourseGiveThisMethodABetterName();

        DocumentFiles = new DocumentFiles(GetDocumentsDirectory());
    }

    public Noark5Archive(InputDiasPackage inputDiasPackage, DirectoryInfo processingDirectory) : base(processingDirectory)
    {
        ArchiveType = ArchiveType.Noark5; // TODO: Get rid of this ...

        Content = inputDiasPackage.WorkingDirectory.ContentWorkDirectory();

        SetupConstructorCommonThingsAndOfCourseGiveThisMethodABetterName();

        InputDiasPackage = inputDiasPackage;
        
        DocumentFiles = new DocumentFiles(InputDiasPackage.TarFile.FullName);
    }
    
    private void SetupConstructorCommonThingsAndOfCourseGiveThisMethodABetterName()
    {
        SetupAddmlXmlUnitAndAddmlInfoAndDetailsAndSoonRenameThisMethod();

        if (AddmlXmlUnit.HasNoDefinedSchema())
            AddmlXmlUnit.Schema = new ArkadeBuiltInXmlSchema(AddmlXsdFileName, Details.ArchiveStandard);

        SetupArchiveXmlUnits();
    }
    
    public ArchiveXmlFile GetArchiveXmlFile(string fileName)
    {
        return XmlUnits.FirstOrDefault(xmlUnit => xmlUnit.File.Name.Equals(fileName))?.File;
    }

    private void SetupArchiveXmlUnits()
    {
        XmlUnits = new List<ArchiveXmlUnit>();

        foreach ((string documentedXmlFileName, IEnumerable<string> documentedXmlSchemas) in Details.DocumentedXmlUnits)
        {
            IEnumerable<ArchiveXmlSchema> userProvidedSchemas =
                documentedXmlSchemas.Select(s => ArchiveXmlSchema.Create(Content.WithFile(s)));

            IEnumerable<ArchiveXmlSchema> arkadeSuppliedSchemas = Details.StandardXmlUnits[documentedXmlFileName]
                .Except(documentedXmlSchemas).Select(s => ArchiveXmlSchema
                    .Create(s, AddmlVersionIsSupported() ? Details.ArchiveStandard : LatestNoark5Version));

            var archiveXmlSchemas = new List<ArchiveXmlSchema>(userProvidedSchemas.Concat(arkadeSuppliedSchemas));

            var archiveXmlFile = new ArchiveXmlFile(Content.WithFile(documentedXmlFileName));

            XmlUnits.Add(new ArchiveXmlUnit(archiveXmlFile, archiveXmlSchemas));
        }
    }

    private bool AddmlVersionIsSupported()
    {
        if (SupportedNoark5Versions.Contains(Details.ArchiveStandard))
            return true;

        Log.Warning(string.Format(Noark5Messages.Noark5VersionNotSupportedForBuiltInSchemas, Details.ArchiveStandard));
        return false;
    }

    public DirectoryInfo GetDocumentsDirectory()
    {
        if (SourceIsTarFile)
            return null;

        if (DocumentsDirectory != null)
            return DocumentsDirectory;

        foreach (DirectoryInfo directory in Content.DirectoryInfo().EnumerateDirectories())
        foreach (string documentDirectoryName in DocumentDirectoryNames)
            if (directory.Name.Equals(documentDirectoryName))
                DocumentsDirectory = directory;

        return DocumentsDirectory ?? DefaultNamedDocumentsDirectory();
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
        return Content.WithSubDirectory(
            DocumentDirectoryNames[0]
        ).DirectoryInfo();
    }

    public override bool IsTestable(out string disqualifyingCause)
    {
        if (!AddmlXmlUnit.File.Exists)
        {
            disqualifyingCause = Noark5Messages.CouldNotFindValidSpecificationFile;
            return false;
        }
        
        disqualifyingCause = null;
        return true;
    }
}
