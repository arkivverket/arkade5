using System;
using System.IO;
using Arkivverket.Arkade.Core.Languages;
using Arkivverket.Arkade.Core.Util;
using Autofac;

namespace Arkivverket.Arkade.Core.Base
{
    /// <summary>
    /// Use this class to interact with the Arkade Api without worrying about dependency injection. This class instantiates Autofac and takes care of all the dependencies.
    /// </summary>
    public class Arkade : IDisposable
    {
        private readonly ArkadeApi _arkadeApi;
        private readonly ArkadeVersion _arkadeVersion;
        private readonly IContainer _container;

        public ArkadeVersion Version() => _arkadeVersion;
        
        public Arkade()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule(new ArkadeAutofacModule());
            _container = builder.Build();

            _container.BeginLifetimeScope();
            _arkadeApi = _container.Resolve<ArkadeApi>();
            _arkadeVersion = _container.Resolve<ArkadeVersion>();
        }

        public void Dispose()
        {
            _container.Dispose();
        }

        public TestSession CreateTestSession(ArchiveDirectory archiveDirectory)
        {
            return _arkadeApi.CreateTestSession(archiveDirectory);
        }

        public TestSession CreateTestSession(ArchiveFile archive)
        {
            return _arkadeApi.CreateTestSession(archive);
        }

        public TestSession RunTests(ArchiveFile archiveFile)
        {
            return _arkadeApi.RunTests(archiveFile);
        }

        public TestSession RunTests(ArchiveDirectory archiveDirectory)
        {
            return _arkadeApi.RunTests(archiveDirectory);
        }

        public void RunTests(TestSession testSession)
        {
            _arkadeApi.RunTests(testSession);
        }

        public void CreatePackage(TestSession testSession, string outputDirectory)
        {
            _arkadeApi.CreatePackage(testSession, outputDirectory);
        }

        public void SaveReport(TestSession testSession, FileInfo file)
        {
            _arkadeApi.SaveReport(testSession, file);
        }

        public void GenerateFileFormatInfoFiles(DirectoryInfo filesDirectory, string resultFileDirectoryPath, string resultFileName, SupportedLanguage language)
        {
            _arkadeApi.GenerateFileFormatInfoFiles(filesDirectory, resultFileDirectoryPath, resultFileName, language);
        }

        public ArchiveType? DetectArchiveType(string archiveFileName)
        {
            return _arkadeApi.DetectArchiveType(archiveFileName);
        }
    }
}