using Arkivverket.Arkade.Identify;
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
        }
    }
}
