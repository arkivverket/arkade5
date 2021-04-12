using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Base.Addml;
using Arkivverket.Arkade.Core.Base.Noark5;
using Arkivverket.Arkade.Core.Base.Siard;
using Arkivverket.Arkade.Core.Identify;
using Arkivverket.Arkade.Core.Logging;
using Arkivverket.Arkade.Core.Metadata;
using Arkivverket.Arkade.Core.Testing;
using Arkivverket.Arkade.Core.Testing.Noark5;
using Arkivverket.Arkade.Core.Util.FileFormatIdentification;
using Autofac;

namespace Arkivverket.Arkade.Core.Util
{
    public class ArkadeAutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<AddmlDatasetTestEngine>().AsSelf();
            builder.RegisterType<AddmlProcessRunner>().AsSelf();
            builder.RegisterType<ArchiveTypeIdentifier>().As<IArchiveTypeIdentifier>();
            builder.RegisterType<FlatFileReaderFactory>().AsSelf();
            builder.RegisterType<Noark5TestEngine>().AsSelf();
            builder.RegisterType<Noark5TestProvider>().AsSelf();
            builder.RegisterType<StatusEventHandler>().As<IStatusEventHandler>().SingleInstance();
            builder.RegisterType<TarCompressionUtility>().As<ICompressionUtility>();
            builder.RegisterType<TestEngineFactory>().AsSelf();
            builder.RegisterType<Noark5TestProvider>().As<ITestProvider>();
            builder.RegisterType<TestSessionFactory>().AsSelf();
            builder.RegisterType<MetadataFilesCreator>().AsSelf();
            builder.RegisterType<DiasMetsCreator>().AsSelf();
            builder.RegisterType<DiasPremisCreator>().AsSelf();
            builder.RegisterType<LogCreator>().AsSelf();
            builder.RegisterType<EadCreator>().AsSelf();
            builder.RegisterType<EacCpfCreator>().AsSelf();
            builder.RegisterType<InfoXmlCreator>().AsSelf();
            builder.RegisterType<InformationPackageCreator>().AsSelf();
            builder.RegisterType<ArkadeApi>().AsSelf();
            builder.RegisterType<TestSessionXmlGenerator>().AsSelf();
            builder.RegisterType<ArkadeVersion>().AsSelf();
            builder.RegisterType<GitHubReleaseInfoReader>().As<IReleaseInfoReader>();
            builder.RegisterType<SiegfriedFileFormatIdentifier>().As<IFileFormatIdentifier>();
            builder.RegisterType<SiegfriedFileInfo>().As<IFileFormatInfo>();
            builder.RegisterType<SiardArchiveReader>().As<ISiardArchiveReader>();
            builder.RegisterType<SiardXmlTableReader>().As<ISiardXmlTableReader>();
        }
    }
}
