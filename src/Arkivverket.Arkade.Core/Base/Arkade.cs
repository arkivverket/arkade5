using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Arkivverket.Arkade.Core.Base.Archives;
using Arkivverket.Arkade.Core.Languages;
using Arkivverket.Arkade.Core.Logging;
using Arkivverket.Arkade.Core.Util;
using Arkivverket.Arkade.Core.Util.ArchiveFormatValidation;
using Arkivverket.Arkade.Core.Util.FileFormatIdentification;
using Autofac;

namespace Arkivverket.Arkade.Core.Base
{
    /// <summary>
    /// Use this class to interact with the Arkade Api without worrying about dependency injection. This class instantiates Autofac and takes care of all the dependencies.
    /// </summary>
    public class Arkade : IDisposable
    {
        private readonly ArkadeApi _arkadeApi;
        private readonly ArkadeCoreApi _arkadeCoreApi;
        private readonly ArkadeVersion _arkadeVersion;
        private readonly IContainer _container;
        private readonly ILifetimeScope _scope;

        public readonly IStatusEventHandler StatusEventHandler;

        public ArkadeVersion Version() => _arkadeVersion;
        
        public Arkade()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule(new ArkadeAutofacModule());
            _container = builder.Build();

            _scope = _container.BeginLifetimeScope();
            _arkadeApi = _container.Resolve<ArkadeApi>();
            _arkadeCoreApi = _container.Resolve<ArkadeCoreApi>();
            _arkadeVersion = _container.Resolve<ArkadeVersion>();
            StatusEventHandler = _container.Resolve<IStatusEventHandler>();
        }

        public void Dispose()
        {
            _scope.Dispose();
            _container.Dispose();
        }

        public Archive LoadArchiveExtraction(FileSystemInfo archiveSource, ArchiveType archiveType)
        {
            return _arkadeCoreApi.LoadArchiveExtraction(archiveSource, archiveType);
        }

        public TestSession CreateTestSession(Archive archive)
        {
            return _arkadeCoreApi.CreateTestSession(archive);
        }

        public void RunTests(Archive archive)
        {
            _arkadeCoreApi.RunTests(archive);
        }

        public void CreatePackage(Archive archive, SupportedLanguage language, bool generateFileFormatInfo, string outputDirectory)
        {
            _arkadeCoreApi.CreatePackage(archive, language, generateFileFormatInfo, outputDirectory);
        }

        public void SaveReport(Archive archive, DirectoryInfo directory, bool standalone, 
            int testResultDisplayLimit)
        {
            //_arkadeApi.SaveReport(archive, directory, standalone, testResultDisplayLimit);
        }

        public IEnumerable<IFileFormatInfo> AnalyseFileFormats(string targetPath, FileFormatScanMode scanMode)
        {
            return _arkadeApi.AnalyseFileFormats(targetPath, scanMode);
        }
        
        public void GenerateFileFormatInfoFiles(Archive archive)
        {
           // _arkadeApi.GenerateFileFormatInfoFiles(archive); // Not in use?
        }

        public void GenerateFileFormatInfoFiles(IEnumerable<IFileFormatInfo> fileFormatInfos, string relativePathRoot, string resultFileFullName, SupportedLanguage language)
        {
            _arkadeApi.GenerateFileFormatInfoFiles(fileFormatInfos, relativePathRoot, resultFileFullName, language);
        }

        public async Task<ArchiveFormatValidationReport> ValidateArchiveFormatAsync(
            FileSystemInfo item, ArchiveFormat format, string resultFileDirectoryPath, SupportedLanguage language)
        {
            return await _arkadeApi.ValidateArchiveFormatAsync(item, format, resultFileDirectoryPath, language);
        }

        public void GenerateMetadataExampleFile(string outputFileName)
        {
            _arkadeApi.GenerateMetadataExampleFile(outputFileName);
        }

        public ArchiveType? DetectArchiveType(string archiveFileName)
        {
            return _arkadeApi.DetectArchiveType(archiveFileName);
        }
    }
}