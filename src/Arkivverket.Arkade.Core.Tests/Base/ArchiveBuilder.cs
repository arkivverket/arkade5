using System;
using System.IO;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Base.Archives;
using Moq;
using static Arkivverket.Arkade.Core.Util.ArkadeConstants;

namespace Arkivverket.Arkade.Core.Tests.Base
{
    /// <summary>
    /// Builds archives for tests. Supports two styles:
    ///  - new ArchiveBuilder(content, processingDirectory).Build&lt;TArchive&gt;()  (renewed-Core style)
    ///  - new ArchiveBuilder().WithArchiveType(..).WithWorkingDirectory*(..).Build()  (legacy fluent style)
    /// </summary>
    public class ArchiveBuilder
    {
        private IArchiveContent _content;
        private DirectoryInfo _processingDirectory;
        private InputDiasPackage _inputDiasPackage;

        private ArchiveType _archiveType = ArchiveType.Noark5;
        private Uuid _uuid = Uuid.Random();
        private DirectoryInfo _contentDirectory;
        private string _archiveFileFullName;

        public ArchiveBuilder()
        {
        }

        public ArchiveBuilder(IArchiveContent content, DirectoryInfo processingDirectory)
        {
            _content = content;
            _processingDirectory = processingDirectory;
        }

        public ArchiveBuilder WithUuid(string uuid)
        {
            Uuid.TryParse(uuid, out _uuid);
            return this;
        }

        public ArchiveBuilder WithUuid(Uuid uuid)
        {
            _uuid = uuid;
            return this;
        }

        // Legacy "working directory root": an Arkade working directory that holds a 'content' sub-directory.
        public ArchiveBuilder WithWorkingDirectoryRoot(string workingDirectory)
        {
            _contentDirectory = new DirectoryInfo(Path.Combine(Resolve(workingDirectory), DirectoryNameContent));
            return this;
        }

        // Legacy "external content": the content directory itself.
        public ArchiveBuilder WithWorkingDirectoryExternalContent(string workingDirectory)
        {
            _contentDirectory = new DirectoryInfo(Resolve(workingDirectory));
            return this;
        }

        public ArchiveBuilder WithWorkingDirectoryExternalContent(DirectoryInfo workingDirectory)
        {
            _contentDirectory = workingDirectory;
            return this;
        }

        public ArchiveBuilder WithArchiveType(ArchiveType archiveType)
        {
            _archiveType = archiveType;
            return this;
        }

        public ArchiveBuilder WithArchiveFileFullName(string archiveFileFullName)
        {
            _archiveFileFullName = archiveFileFullName;
            return this;
        }

        // NOTE: renewed Core derives Archive.Details from the parsed ADDML at construction (init-only),
        // so a forced standard version is no longer honoured here. Kept so callers compile; revisit if a
        // test genuinely needs to override the archive standard. (B4 tail)
        public ArchiveBuilder WithArchiveDetails(string standardVersion)
        {
            return this;
        }

        public ArchiveBuilder WithInputDiasPackage(ArchiveMetadata archiveMetadata)
        {
            _inputDiasPackage = new Mock<InputDiasPackage>().Object;
            return this;
        }

        public T Build<T>() where T : Archive
        {
            return (T)Activator.CreateInstance(typeof(T), ResolveContent(), ResolveProcessingDirectory(), _inputDiasPackage);
        }

        public Archive Build()
        {
            Type archiveClrType = _archiveType switch
            {
                ArchiveType.Noark3 => typeof(Noark3Archive),
                ArchiveType.Noark4 => typeof(Noark4Archive),
                ArchiveType.Noark5 => typeof(Noark5Archive),
                ArchiveType.SpecializedSystem => typeof(SpecializedSystemArchive),
                ArchiveType.Siard => typeof(SiardArchive),
                _ => throw new ArgumentOutOfRangeException(nameof(_archiveType), _archiveType, null)
            };

            return (Archive)Activator.CreateInstance(archiveClrType, ResolveContent(), ResolveProcessingDirectory(), _inputDiasPackage);
        }

        private IArchiveContent ResolveContent()
        {
            if (_content != null)
                return _content;

            if (_contentDirectory == null)
                throw new InvalidOperationException(
                    "Archive content is required — supply it via the constructor or WithWorkingDirectory*.");

            return new DirectoryArchiveContent(_contentDirectory);
        }

        private DirectoryInfo ResolveProcessingDirectory()
        {
            if (_processingDirectory != null)
                return _processingDirectory;

            var directory = new DirectoryInfo(Path.Combine(Path.GetTempPath(), "arkade-tests", _uuid.ToString()));
            directory.Create();
            return directory;
        }

        private static string Resolve(string path)
        {
            path = path.Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar);
            return Path.IsPathRooted(path) ? path : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);
        }
    }
}
