using System;
using System.IO;
using System.Reflection;
using Arkivverket.Arkade.Core.Base.Archives;
using Arkivverket.Arkade.Core.Util;
using Serilog;
using static Arkivverket.Arkade.Core.Util.ArkadeConstants;

namespace Arkivverket.Arkade.Core.Base
{
    public class DiasPackageWorkingDirectory
    {
        private static readonly ILogger Log = Serilog.Log.ForContext(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly ArkadeDirectory _root;
        //private readonly ArkadeDirectory _externalContentDirectory;

        public DiasPackageWorkingDirectory(DirectoryInfo root)
        {
            _root = new ArkadeDirectory(root);
        }
        //public DiasPackageWorkingDirectory(DirectoryInfo root, DirectoryInfo externalContentDirectory)
        //{
        //    _root = new ArkadeDirectory(root);
        //    if (externalContentDirectory != null)
        //    {
        //        _externalContentDirectory = new ArkadeDirectory(externalContentDirectory);
        //        Log.Debug("Setting up working directory with external content directory: " + externalContentDirectory.FullName);
        //    }
        //}

        //public static DiasPackageWorkingDirectory FromArchiveFile()
        //{
        //    return FromExternalDirectory(null);
        //}

        ///// <summary>
        ///// Initializes a new working directory for this archive using the timestamp as the identifier. An empty folder structure is created on disk.
        ///// </summary>
        ///// <param name="externalContentDirectory">optional external content directory</param>
        ///// <returns></returns>
        //internal static DiasPackageWorkingDirectory FromExternalDirectory(DirectoryInfo externalContentDirectory)
        //{
        //    if (ArkadeProcessingArea.WorkDirectory == null)
        //        throw new IOException(Resources.ExceptionMessages.ArkadeProcessAreaNotSet);

        //    string dateString = DateTime.Now.ToString("yyyyMMddHHmmss");
        //    var rootDirectory = new DirectoryInfo(
        //        Path.Combine(ArkadeProcessingArea.WorkDirectory.FullName, dateString)
        //    );

        //    var workingDirectory = new DiasPackageWorkingDirectory(rootDirectory, externalContentDirectory);
        //    workingDirectory.CreateAllFolders();
        //    return workingDirectory;
        //}

        public ArkadeDirectory Root()
        {
            return _root;
        }

        /// <summary>
        /// This directory contains the archive files. Can be an external content directory located outside of Arkade's work directory.
        /// </summary>
        /// <returns></returns>
        //public ArkadeDirectory Content()
        //{
        //    if (HasExternalContentDirectory())
        //        return _externalContentDirectory;
        //    return _root.WithSubDirectory("content");
        //}

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

        public void CreateDirectories(PackageType packageType)
        {
            //Root().Create();
            DescriptiveMetadata().Create();
            AdministrativeMetadata().Create();
            if (packageType == PackageType.ArchivalInformationPackage)
                RepositoryOperations().Create();
            ContentWorkDirectory().Create();
        }

        //public bool HasExternalContentDirectory()
        //{
        //    return _externalContentDirectory != null;
        //}

        public long GetSize()
        {
            // Returns the size of the files staged in the working directory (package metadata and
            // similar). NOTE: this does NOT include the archive content/document files - those are
            // streamed straight into the package TAR from the source (Archive.Content) at packaging
            // time and are never staged under Root. Since that content is normally the bulk of the
            // package, this is a significant underestimate of the final package size.
            return Root().GetSize();
        }
    }
}
