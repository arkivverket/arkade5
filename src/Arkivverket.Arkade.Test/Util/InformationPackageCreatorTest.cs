using System.IO;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Util;
using Xunit;

namespace Arkivverket.Arkade.Test.Util
{
    public class InformationPackageCreatorTest
    {
        [Fact]
        public void ShouldCreateSip()
        {
            var archive = new Archive(ArchiveType.Noark5, Uuid.Random(), new WorkingDirectory(new DirectoryInfo(@"c:\temp\arkade-input\")));
            var targetFileName = @"c:\temp\arkade-output\package.tar";

            var creator = new InformationPackageCreator();
            creator.CreateSip(archive, targetFileName);


        }
    }
}