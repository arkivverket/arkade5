using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Identify;
using Arkivverket.Arkade.Tests;
using Autofac;

namespace Arkivverket.Arkade.Util
{
    public class ArkadeAutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ArchiveExtractor>().As<IArchiveExtractor>();
            builder.RegisterType<TarCompressionUtility>().As<ICompressionUtility>();
            builder.RegisterType<ArchiveExtractionReader>().AsSelf();
            builder.RegisterType<ArchiveIdentifier>().As<IArchiveIdentifier>();
            builder.RegisterType<TestEngine>().AsSelf().SingleInstance();
            builder.RegisterType<ArchiveContentReader>().As<IArchiveContentReader>();
            builder.RegisterType<TestProvider>().As<ITestProvider>();
        }
    }
}
