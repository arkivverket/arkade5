using System;
using System.IO;
using Arkivverket.Arkade.Util;
using Autofac;

namespace Arkivverket.Arkade.Core
{
    /// <summary>
    /// Use this class to interact with the Arkade Api without worrying about dependency injection. This class instantiates Autofac and takes care of all the dependencies.
    /// </summary>
    public class Arkade : IDisposable
    {
        private readonly ArkadeApi _arkadeApi;
        private readonly IContainer _container;

        public Arkade()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule(new ArkadeAutofacModule());
            _container = builder.Build();

            _container.BeginLifetimeScope();
            _arkadeApi = _container.Resolve<ArkadeApi>();
        }

        public void Dispose()
        {
            _container.Dispose();
        }

        public TestSession RunTests(ArchiveFile archiveFile)
        {
            return _arkadeApi.RunTests(archiveFile);
        }

        public TestSession RunTests(ArchiveDirectory archiveDirectory)
        {
            return _arkadeApi.RunTests(archiveDirectory);
        }

        public void CreatePackage(TestSession testSession, PackageType packageType)
        {
            _arkadeApi.CreatePackage(testSession, packageType);
        }

        public void SaveReport(TestSession testSession, FileInfo file)
        {
            _arkadeApi.SaveReport(testSession, file);
        }
    }
}