using System;
using System.IO;
using Arkivverket.Arkade.Core.Base.Addml;
using Arkivverket.Arkade.Core.Base.Addml.Definitions;
using Arkivverket.Arkade.Core.Util;
using static Arkivverket.Arkade.Core.Util.ArkadeConstants;

namespace Arkivverket.Arkade.Core.Base
{
    public abstract class Archive
    {
        public DirectoryInfo ProcessingDirectory { get; private init; }

        public bool SourceIsTarFile => InputDiasPackage?.TarFile != null;

        public ArkadeDirectory Content { get; protected init; }

        public ArchiveContent WipContent { get; }
        public InputDiasPackage InputDiasPackage { get; protected init; }
        public OutputDiasPackage OutputDiasPackage { get; set; }
        public ArchiveType ArchiveType { get; protected init; } // TODO: Consider to liquidate
        public AddmlXmlUnit AddmlXmlUnit { get; protected set; }
        public AddmlInfo AddmlInfo { get; protected set; }
        public IArchiveDetails Details { get; protected set; }
        public TestSession TestSession { get; set; }
        public abstract bool IsTestable(out string disqualifyingCause);

        protected Archive(DirectoryInfo processingDirectory)
        {
            ProcessingDirectory = processingDirectory;
        }

        protected void SetupAddmlXmlUnitAndAddmlInfoAndDetailsAndSoonRenameThisMethod()
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
    }

    [Flags]
    public enum ArchiveType
    {
        Noark3,
        Noark4,
        Noark5,
        Fagsystem,
        Siard,
    }
}