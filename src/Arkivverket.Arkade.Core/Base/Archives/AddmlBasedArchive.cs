using System.IO;
using Arkivverket.Arkade.Core.Base.Addml;
using Arkivverket.Arkade.Core.Base.Addml.Definitions;
using Serilog;
using static Arkivverket.Arkade.Core.Base.ArkadeBuiltInXmlSchema;
using static Arkivverket.Arkade.Core.Util.ArkadeConstants;

namespace Arkivverket.Arkade.Core.Base.Archives;

public abstract class AddmlBasedArchive : Archive
{
    public AddmlXmlUnit AddmlXmlUnit { get; protected init; }
    public AddmlInfo AddmlInfo { get; protected init; }

    protected AddmlBasedArchive(ArchiveContent content, DirectoryInfo processingDirectory, InputDiasPackage inputDiasPackage = null)
        : base(content, processingDirectory, inputDiasPackage)
    {
        if (Content.GetFile(AddmlXmlFileName) is not { } addmlFileInfo)
        {
            Log.Error("No addml file found in archive.");
            return;
        }

        AddmlXmlUnit = SetupAddmlXmlUnit(addmlFileInfo);
        AddmlInfo = AddmlUtil.ReadFromFile(AddmlXmlUnit.File.FullName, AddmlXmlUnit.Schema.AsStream());
        Details = new ArchiveDetails(AddmlInfo.Addml);
    }

    protected AddmlXmlUnit SetupAddmlXmlUnit(FileInfo addmlFileInfo)
    {
        var addmlXmlFile = new ArchiveXmlFile(addmlFileInfo);

        FileInfo addmlXsdFileInfo = Content.GetFile(AddmlXsdFileName);

        ArchiveXmlSchema addmlSchema = addmlXsdFileInfo.Exists
            ? new UserProvidedXmlSchema(addmlXsdFileInfo)
            : new ArkadeBuiltInXmlSchema(AddmlXsdFileName, new Version(BuiltInAddmlSchemaVersion));

        return new AddmlXmlUnit(addmlXmlFile, addmlSchema);
    }
}
