using System.IO;
using Arkivverket.Arkade.Core.Base.Addml;
using Arkivverket.Arkade.Core.Base.Addml.Definitions;
using Serilog;
using static Arkivverket.Arkade.Core.Base.ArkadeBuiltInXmlSchema;
using static Arkivverket.Arkade.Core.Util.ArkadeConstants;

namespace Arkivverket.Arkade.Core.Base.Archives;

public abstract class AddmlBasedArchive : Archive
{
    public new readonly DirectoryArchiveContent Content;
    public AddmlXmlUnit AddmlXmlUnit { get; protected init; }
    public AddmlInfo AddmlInfo { get; protected init; }

    protected AddmlBasedArchive(DirectoryArchiveContent content, DirectoryInfo processingDirectory, InputDiasPackage inputDiasPackage = null)
        : base(content, processingDirectory, inputDiasPackage)
    {
        Content = content;

        if (Content.GetFile(AddmlXmlFileName) is not { } addmlFile)
        {
            Log.Error("No addml file found in archive.");
            return;
        }

        AddmlXmlUnit = SetupAddmlXmlUnit(addmlFile);
        AddmlInfo = AddmlUtil.ReadFromFile(AddmlXmlUnit.File.FullName, AddmlXmlUnit.Schema.AsStream());
        Details = new ArchiveDetails(AddmlInfo.Addml);
    }

    protected AddmlXmlUnit SetupAddmlXmlUnit(FileInfo addmlFile)
    {
        var addmlXmlFile = new ArchiveXmlFile(addmlFile);

        ArchiveXmlSchema addmlSchema = Content.GetFile(AddmlXsdFileName) is { } addmlXsdFile
            ? new UserProvidedXmlSchema(addmlXsdFile)
            : new ArkadeBuiltInXmlSchema(AddmlXsdFileName, new Version(BuiltInAddmlSchemaVersion));

        return new AddmlXmlUnit(addmlXmlFile, addmlSchema);
    }
}
