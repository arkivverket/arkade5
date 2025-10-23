using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Logging;
using Arkivverket.Arkade.Core.Resources;
using Serilog;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Arkivverket.Arkade.Core.Util;
using ICSharpCode.SharpZipLib.Tar;
using static Arkivverket.Arkade.Core.Util.ArkadeConstants;

namespace Arkivverket.Arkade.Core;

public class Noark5Archive : Archive
{
    public List<ArchiveXmlUnit> XmlUnits { get; private set; }
    internal DocumentFiles DocumentFiles { get; init; }
    private DirectoryInfo DocumentsDirectory { get; set; }
    private string DocumentsDirectoryName { get; set; }

    public Noark5Archive(DirectoryInfo contentDirectory, DirectoryInfo processingDirectory, IStatusEventHandler statusEventHandler, InputDiasPackage inputDiasPackage) :
        base(ArchiveType.Noark5, contentDirectory, processingDirectory, statusEventHandler, inputDiasPackage)
    {
        if (AddmlXmlUnit.HasNoDefinedSchema())
            AddmlXmlUnit.Schema = new ArkadeBuiltInXmlSchema(AddmlXsdFileName, Details.ArchiveStandard);

        SetupArchiveXmlUnits();

        DocumentFiles = InputDiasPackage == null
            ? new DocumentFiles(GetDocumentsDirectory())
            : new DocumentFiles(InputDiasPackage.TarFile.FullName);
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
}
