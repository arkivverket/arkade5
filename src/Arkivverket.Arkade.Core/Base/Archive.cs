using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Arkivverket.Arkade.Core.Base.Addml;
using Arkivverket.Arkade.Core.Base.Addml.Definitions;
using Arkivverket.Arkade.Core.Base.Siard;
using Arkivverket.Arkade.Core.ExternalModels.Metadata;
using Arkivverket.Arkade.Core.Logging;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Util;
using Serilog;
using static Arkivverket.Arkade.Core.Util.ArkadeConstants;

namespace Arkivverket.Arkade.Core.Base
{
    public abstract class Archive
    {
        private static readonly ILogger Log = Serilog.Log.ForContext(MethodBase.GetCurrentMethod().DeclaringType);
        private static IStatusEventHandler _statusEventHandler;

        public DirectoryInfo ProcessingDirectory { get; }

        public bool SourceIsTarFile => InputDiasPackage?.TarFile != null;
        public ArkadeDirectory Content { get; }
        //public ArchiveContent Content { get; }
        public InputDiasPackage InputDiasPackage { get; }
        public OutputDiasPackage OutputDiasPackage { get; set; }
        public ArchiveType ArchiveType { get; }

        public AddmlXmlUnit AddmlXmlUnit { get; }
        public AddmlInfo AddmlInfo { get; }
        public IArchiveDetails Details { get; }
        public TestSession TestSession { get; set; }

        protected Archive(ArchiveType archiveType, DirectoryInfo archiveExtractionDirectory,
            DirectoryInfo processingDirectory, IStatusEventHandler statusEventHandler,InputDiasPackage inputDiasPackage = null)
        {
            _statusEventHandler = statusEventHandler;

            ArchiveType = archiveType;

            Content = new ArkadeDirectory(archiveExtractionDirectory);
            
            ProcessingDirectory = processingDirectory;

            InputDiasPackage = inputDiasPackage;
            
            if (archiveType == ArchiveType.Siard)
            {
                Details = SetupSiardArchiveDetails(Content);
                return;
            }
            
            AddmlXmlUnit = SetupAddmlXmlUnit();

            if (!AddmlXmlUnit.File.Exists)
                return;

            using Stream xmlSchemaStream = AddmlXmlUnit.HasNoDefinedSchema()
                ? ResourceUtil.GetResourceAsStream(AddmlXsdResource)
                : AddmlXmlUnit.Schema.AsStream();

            AddmlInfo = AddmlUtil.ReadFromFile(AddmlXmlUnit.File.FullName, xmlSchemaStream);

            Details = new ArchiveDetails(AddmlInfo.Addml);
        }

        private static IArchiveDetails SetupSiardArchiveDetails(ArkadeDirectory content)
        {
            FileInfo siardArchiveFile = content.DirectoryInfo().GetFiles("*.siard").FirstOrDefault();
            if (siardArchiveFile == null)
                throw new ArkadeException("Siard file not found");
            if (!siardArchiveFile.Exists)
                throw new ArkadeException(string.Format(ExceptionMessages.FileNotFound, siardArchiveFile.FullName));

            if (new SiardArchiveReader().TryDeserializeToSiard2_1(siardArchiveFile.FullName, out siardArchive siard2Archive, out string errorMessage))
                return new SiardArchiveDetails(siard2Archive);

            _statusEventHandler?.RaiseEventOperationMessage(null,
                string.Format(SiardMessages.DeserializationUnsuccessfulMessage, SiardMetadataXmlFileName, "2.1", errorMessage),
                OperationMessageStatus.Error);

            return null;
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