using System;
using System.IO;
using System.Reflection;
using Arkivverket.Arkade.Core.Util;
using Serilog;
using static Arkivverket.Arkade.Core.Util.ArkadeConstants;

namespace Arkivverket.Arkade.Core.Base
{
    public class WorkingDirectory
    {
        private static readonly ILogger Log = Serilog.Log.ForContext(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly ArkadeDirectory _root;
        private readonly ArkadeDirectory _externalContentDirectory;

        public WorkingDirectory(DirectoryInfo root)
        {
            _root = new ArkadeDirectory(root);
        }
        public WorkingDirectory(DirectoryInfo root, DirectoryInfo externalContentDirectory)
        {
            _root = new ArkadeDirectory(root);
            if (externalContentDirectory != null)
            {
                _externalContentDirectory = new ArkadeDirectory(externalContentDirectory);
                Log.Debug("Setting up working directory with external content directory: " + externalContentDirectory.FullName);
            }
        }

        public static WorkingDirectory FromArchiveFile()
        {
            return FromExternalDirectory(null);
        }

        /// <summary>
        /// Initializes a new working directory for this archive using the timestamp as the identifier. An empty folder structure is created on disk.
        /// </summary>
        /// <param name="externalContentDirectory">optional external content directory</param>
        /// <returns></returns>
        internal static WorkingDirectory FromExternalDirectory(DirectoryInfo externalContentDirectory)
        {
            if (ArkadeProcessingArea.WorkDirectory == null)
                throw new IOException(Resources.ExceptionMessages.ArkadeProcessAreaNotSet);

            string dateString = DateTime.Now.ToString("yyyyMMddHHmmss");
            var rootDirectory = new DirectoryInfo(
                Path.Combine(ArkadeProcessingArea.WorkDirectory.FullName, dateString)
            );

            var workingDirectory = new WorkingDirectory(rootDirectory, externalContentDirectory);
            workingDirectory.CreateAllFolders();
            return workingDirectory;
        }

        public ArkadeDirectory Root()
        {
            return _root;
        }

        /// <summary>
        /// This directory contains the archive files. Can be an external content directory located outside of Arkade's work directory.
        /// </summary>
        /// <returns></returns>
        public ArkadeDirectory Content()
        {
            if (HasExternalContentDirectory())
                return _externalContentDirectory;
            return _root.WithSubDirectory("content");
        }

        /// <summary>
        /// This is the local content directory inside Arkade's work directory. Writing of content files (like the addml.xml) should done within this directory.
        /// </summary>
        /// <returns></returns>
        public ArkadeDirectory ContentWorkDirectory()
        {
            return _root.WithSubDirectory("content");
        }

        public ArkadeDirectory DescriptiveMetadata()
        {
            return _root.WithSubDirectory("descriptive_metadata");
        }

        public ArkadeDirectory AdministrativeMetadata()
        {
            return _root.WithSubDirectory("administrative_metadata");
        }

        public ArkadeDirectory RepositoryOperations()
        {
            return AdministrativeMetadata().WithSubDirectory(ArkadeConstants.DirectoryNameRepositoryOperations);
        }

        private void CreateAllFolders()
        {
            Root().Create();
            DescriptiveMetadata().Create();
            AdministrativeMetadata().Create();
            RepositoryOperations().Create();
            ContentWorkDirectory().Create();
        }

        public bool HasExternalContentDirectory()
        {
            return _externalContentDirectory != null;
        }

        public void EnsureAdministrativeMetadataHasAddmlFiles(string addmlFileName)
        {
            TryCopyAddmlFileToAdministrativeMetadata(addmlFileName);

            if (!TryCopyAddmlFileToAdministrativeMetadata(AddmlXsdFileName))
            {
                AdministrativeMetadata().AddFileFromResources(AddmlXsdResource, AddmlXsdFileName);
                Log.Debug($"Adding {AddmlXsdFileName} from Arkade built-in resources to administrative_metadata.");
            }
        }

        private bool TryCopyAddmlFileToAdministrativeMetadata(string addmlFileName)
        {
            FileInfo targetAddmlFile = AdministrativeMetadata().WithFile(addmlFileName);

            if (targetAddmlFile.Exists)
                return false;

            FileInfo contentAddml = Content().WithFile(addmlFileName);

            if (!contentAddml.Exists)
                return false;

            Log.Debug($"Copying ADDML file {contentAddml.FullName} to administrative_metadata.");
            contentAddml.CopyTo(targetAddmlFile.FullName);
            return true;
        }

        public long GetSize()
        {
            return Root().GetSize() + Content().GetSize();
        }
    }
}
