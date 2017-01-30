using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Core.Addml;
using Arkivverket.Arkade.Core.Noark5;
using Arkivverket.Arkade.Identify;
using Arkivverket.Arkade.Logging;
using Arkivverket.Arkade.Metadata;
using Arkivverket.Arkade.Tests;
using Arkivverket.Arkade.Tests.Noark5;
using Autofac;

namespace Arkivverket.Arkade.Util
{
    public class ArkadeAutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<AddmlDatasetTestEngine>().AsSelf();
            builder.RegisterType<AddmlProcessRunner>().AsSelf();
            builder.RegisterType<ArchiveContentReader>().As<IArchiveContentReader>();
            builder.RegisterType<ArchiveIdentifier>().As<IArchiveIdentifier>();
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
            builder.RegisterType<EadCreator>().AsSelf();
            builder.RegisterType<EacCpfCreator>().AsSelf();
            builder.RegisterType<InformationPackageCreator>().AsSelf();
            builder.RegisterType<ArkadeApi>().AsSelf();
            builder.RegisterType<TestSessionXmlGenerator>().AsSelf();
        }
    }
}
