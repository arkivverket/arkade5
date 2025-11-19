using System.IO;
using Arkivverket.Arkade.Core.Base.Addml;
using Arkivverket.Arkade.Core.Base.Addml.Definitions;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Util;
using static Arkivverket.Arkade.Core.Util.ArkadeConstants;

namespace Arkivverket.Arkade.Core.Base.Archives;

public abstract class AddmlBasedArchive : Archive
{
    public AddmlXmlUnit AddmlXmlUnit { get; protected set; }
    public AddmlInfo AddmlInfo { get; protected set; }
    
    protected AddmlBasedArchive(DirectoryInfo processingDirectory) : base(processingDirectory)
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
        FileInfo addmlFileInfo = Content?.WithFile(AddmlXmlFileName);

        // .................Move to Noark5Archive.......................
        if (!addmlFileInfo.Exists && ArchiveType == ArchiveType.Noark5)
            addmlFileInfo = Content.WithFile(ArkivuttrekkXmlFileName);
        // .............................................................

        var addmlXmlFile = new ArchiveXmlFile(addmlFileInfo);

        FileInfo addmlXsdFileInfo = Content.WithFile(AddmlXsdFileName);

        ArchiveXmlSchema addmlSchema = addmlXsdFileInfo.Exists
            ? ArchiveXmlSchema.Create(addmlXsdFileInfo)
            : null;

        return new AddmlXmlUnit(addmlXmlFile, addmlSchema);
    }
    
    public override bool IsTestable(out string disqualifyingCause)
    {
        if (TestSession.AddmlDefinition == null) // TODO: Follow up this
        {
            disqualifyingCause = Noark5Messages.CouldNotFindValidSpecificationFile;
            return false;
        }
        
        disqualifyingCause = null;
        return true;
    }   
}
