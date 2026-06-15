using System.IO;
using Arkivverket.Arkade.Core.Util;
using static Arkivverket.Arkade.Core.Util.ArkadeConstants;

namespace Arkivverket.Arkade.Core.Base
{
    public class DiasPackageWorkingDirectory
    {
        private readonly ArkadeDirectory _root;

        public DiasPackageWorkingDirectory(DirectoryInfo root)
        {
            _root = new ArkadeDirectory(root);
        }

        public ArkadeDirectory Root()
        {
            return _root;
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

        public void CreateDirectories(PackageType packageType)
        {
            //Root().Create();
            DescriptiveMetadata().Create();
            AdministrativeMetadata().Create();
            if (packageType == PackageType.ArchivalInformationPackage)
                RepositoryOperations().Create();
            ContentWorkDirectory().Create();
        }

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
