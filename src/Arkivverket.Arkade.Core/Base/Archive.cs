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
        public DirectoryInfo ProcessingDirectory { get; }

        public bool SourceIsTarFile => InputDiasPackage?.TarFile != null;

        public ArkadeDirectory Content { get; protected init; }

        //public ArchiveContent Content { get; }
        public InputDiasPackage InputDiasPackage { get; }
        public OutputDiasPackage OutputDiasPackage { get; set; }
        public ArchiveType ArchiveType { get; }

        public AddmlXmlUnit AddmlXmlUnit { get; }
        public AddmlInfo AddmlInfo { get; }
        public IArchiveDetails Details { get; protected init; }
        public TestSession TestSession { get; set; }
        public abstract bool IsTestable(out string disqualifyingCause);

        protected Archive(ArchiveType archiveType, InputDiasPackage inputDiasPackage = null)
        {
            ArchiveType = archiveType;

            //Content = new ArkadeDirectory(archiveExtractionDirectory);

            ProcessingDirectory = CreateProcessingDirectory();

            InputDiasPackage = inputDiasPackage;

            AddmlXmlUnit = SetupAddmlXmlUnit();

            if (!AddmlXmlUnit.File.Exists)
                return;

            using Stream xmlSchemaStream = AddmlXmlUnit.HasNoDefinedSchema()
                ? ResourceUtil.GetResourceAsStream(AddmlXsdResource)
                : AddmlXmlUnit.Schema.AsStream();

            AddmlInfo = AddmlUtil.ReadFromFile(AddmlXmlUnit.File.FullName, xmlSchemaStream);

            Details = new ArchiveDetails(AddmlInfo.Addml);
        }

        private AddmlXmlUnit SetupAddmlXmlUnit()
        {
            FileInfo addmlFileInfo = Content.WithFile(AddmlXmlFileName);

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

        private static DirectoryInfo CreateProcessingDirectory()
        {
            string workDirectoryFullName = ArkadeProcessingArea.WorkDirectory.FullName;
            var nowTimeStampString = DateTime.Now.ToString("yyyyMMddHHmmss");

            var processingDirectory = new DirectoryInfo(Path.Combine(workDirectoryFullName, nowTimeStampString));

            processingDirectory.Create();

            return processingDirectory;
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