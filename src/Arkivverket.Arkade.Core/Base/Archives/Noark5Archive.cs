using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Arkivverket.Arkade.Core.Base.Addml.Definitions;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Util;
using ICSharpCode.SharpZipLib.Tar;
using Serilog;
using static Arkivverket.Arkade.Core.Base.ArkadeBuiltInXmlSchema;
using static Arkivverket.Arkade.Core.Util.ArkadeConstants;

namespace Arkivverket.Arkade.Core.Base.Archives;

public sealed class Noark5Archive : AddmlBasedArchive
{
    
    private static readonly ILogger Log = Serilog.Log.ForContext(MethodBase.GetCurrentMethod().DeclaringType);
    public List<ArchiveXmlUnit> XmlUnits { get; private set; }
    internal DocumentFiles DocumentFiles { get; init; }
    private DirectoryInfo DocumentsDirectory { get; set; }
    private string DocumentsDirectoryName { get; set; }

    [SetsRequiredMembers]
    public Noark5Archive(DirectoryInfo archiveExtractionDirectory, DirectoryInfo processingDirectory) : base(processingDirectory, SetupContent(archiveExtractionDirectory))
    {
        AddmlXmlUnit = new AddmlXmlUnit(null, null); // TODO: Implement!
        AddmlInfo = new AddmlInfo(null, null); // TODO: Implement!
        
        ArchiveType = ArchiveType.Noark5; // TODO: Get rid of this ...
        
        SetupConstructorCommonThingsAndOfCourseGiveThisMethodABetterName();

        DocumentFiles = new DocumentFiles(GetDocumentsDirectory());
    }

    [SetsRequiredMembers]
    public Noark5Archive(InputDiasPackage inputDiasPackage, DirectoryInfo processingDirectory) : base(processingDirectory, SetupContent(inputDiasPackage))
    {
        AddmlXmlUnit = new AddmlXmlUnit(null, null); // TODO: Implement!
        AddmlInfo = new AddmlInfo(null, null); // TODO: Implement!
        
        ArchiveType = ArchiveType.Noark5; // TODO: Get rid of this ...

        SetupConstructorCommonThingsAndOfCourseGiveThisMethodABetterName();

        DocumentFiles = new DocumentFiles(InputDiasPackage.TarFile.FullName);
    }
    
    private void SetupConstructorCommonThingsAndOfCourseGiveThisMethodABetterName()
    {
        // if (AddmlXmlUnit.HasNoDefinedSchema())
        //     AddmlXmlUnit.Schema = new ArkadeBuiltInXmlSchema(AddmlXsdFileName, Details.ArchiveStandard);

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
                documentedXmlSchemas.Select(s => new UserProvidedXmlSchema(Content.GetFile(s)));

            string archiveTypeVersion = AddmlVersionIsSupported() ? Details.ArchiveStandard : LatestNoark5Version;
            string pathCompatibleVersionString = "v" + archiveTypeVersion.Replace('.', '_');
            var xsdResourceLocalPath = $"{string.Format(LocalDirectoryPathNoark5XsdResources, pathCompatibleVersionString)}";

            IEnumerable<ArchiveXmlSchema> arkadeSuppliedSchemas = Details.StandardXmlUnits[documentedXmlFileName]
                .Except(documentedXmlSchemas).Select(schemaName =>
                    new ArkadeBuiltInXmlSchema(schemaName, new Version(archiveTypeVersion, xsdResourceLocalPath)));

            var archiveXmlSchemas = new List<ArchiveXmlSchema>(userProvidedSchemas.Concat(arkadeSuppliedSchemas));

            var archiveXmlFile = new ArchiveXmlFile(Content.GetFile(documentedXmlFileName));

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


        foreach (string documentDirectoryName in DocumentDirectoryNames)
        {
            if (Content.GetDirectory(documentDirectoryName) is not { } foundDocumentsDirectory)
                continue;

            DocumentsDirectory = foundDocumentsDirectory;
            return DocumentsDirectory;
        }

        return null;
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
        return /*Content.WithSubDirectory(*/
            new DirectoryInfo(DocumentDirectoryNames[0]);
        //).DirectoryInfo();
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
