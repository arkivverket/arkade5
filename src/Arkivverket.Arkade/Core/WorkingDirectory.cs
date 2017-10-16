using System;
using System.IO;
using System.Reflection;
using Arkivverket.Arkade.Util;
using Serilog;

namespace Arkivverket.Arkade.Core
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

        public static WorkingDirectory FromUuid(Uuid uuid)
        {
            return FromUuid(uuid, null);
        }

        /// <summary>
        /// Initializes a new working directory for this archive using the uuid and timestamp as the identifier. An empty folder structure is created on disk.
        /// </summary>
        /// <param name="uuid">the archive uuid</param>
        /// <param name="externalContentDirectory">optional external content directory</param>
        /// <returns></returns>
        internal static WorkingDirectory FromUuid(Uuid uuid, DirectoryInfo externalContentDirectory)
        {
            string dateString = DateTime.Now.ToString("yyyyMMddHHmmss");
            var rootDirectory = new DirectoryInfo(
                ArkadeProcessingArea.WorkDirectory.FullName +
                Path.DirectorySeparatorChar + dateString + "-" + uuid.GetValue());

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

        public void CopyAddmlFileToAdministrativeMetadata()
        {
            FileInfo targetAddmlFile = AdministrativeMetadata().WithFile(ArkadeConstants.AddmlXmlFileName);
            if (!targetAddmlFile.Exists)
            {
                FileInfo contentAddml = Content().WithFile(ArkadeConstants.AddmlXmlFileName);
                if (contentAddml.Exists)
                {
                    Log.Information($"Copying ADDML file {contentAddml.FullName} to administrative_metadata.");
                    contentAddml.CopyTo(targetAddmlFile.FullName);
                }
                else
                {
                    FileInfo arkivuttrekkAddml = Content().WithFile(ArkadeConstants.ArkivuttrekkXmlFileName);
                    if (arkivuttrekkAddml.Exists)
                    {
                        Log.Information($"Copying ADDML file {arkivuttrekkAddml.FullName} to administrative_metadata.");
                        arkivuttrekkAddml.CopyTo(targetAddmlFile.FullName);
                    }
                }

            }
        }
    }
}
