using System.IO;
using System.Linq;
using Arkivverket.Arkade.Core.Base.Addml;
using Arkivverket.Arkade.Core.Base.Addml.Definitions;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Util;
using static Arkivverket.Arkade.Core.Base.ArkadeBuiltInXmlSchema;
using static Arkivverket.Arkade.Core.Util.ArkadeConstants;

namespace Arkivverket.Arkade.Core.Base.Archives;

public abstract class AddmlBasedArchive : Archive
{
    public required AddmlXmlUnit AddmlXmlUnit { get; init; }
    public required AddmlInfo AddmlInfo { get; init; }
    
    protected AddmlBasedArchive(ArchiveContent content, DirectoryInfo processingDirectory) : base(content, processingDirectory)
    {
        AddmlXmlUnit = SetupAddmlXmlUnit();

        if (!AddmlXmlUnit.File.Exists)
            return;

        using Stream xmlSchemaStream = AddmlXmlUnit.HasNoDefinedSchema()
            ? ResourceUtil.GetResourceAsStream(AddmlXsdResource)
            : AddmlXmlUnit.Schema.AsStream();

        AddmlInfo = AddmlUtil.ReadFromFile(AddmlXmlUnit.File.FullName, xmlSchemaStream);

        Details = new ArchiveDetails(AddmlInfo.Addml);
    }

    protected AddmlXmlUnit SetupAddmlXmlUnit()
    {
        var addmlFileInfo = Content.GetFile(AddmlXmlFileName);

        // .................Move to Noark5Archive.......................
        if (!addmlFileInfo.Exists && ArchiveType == ArchiveType.Noark5)
            addmlFileInfo = Content.GetFile(ArkivuttrekkXmlFileName);
        // .............................................................

        var addmlXmlFile = new ArchiveXmlFile(addmlFileInfo);

        FileInfo addmlXsdFileInfo = Content.GetFile(AddmlXsdFileName);

        ArchiveXmlSchema addmlSchema = addmlXsdFileInfo.Exists
            ? new UserProvidedXmlSchema(addmlXsdFileInfo)
            : new ArkadeBuiltInXmlSchema(AddmlXsdFileName, new Version(BuiltInAddmlSchemaVersion));
        
        return new AddmlXmlUnit(addmlXmlFile, addmlSchema);
    }
}
