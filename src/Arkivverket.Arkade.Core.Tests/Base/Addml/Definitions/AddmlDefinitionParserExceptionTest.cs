using System;
using System.Collections.Generic;
using System.IO;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Base.Addml;
using Arkivverket.Arkade.Core.Base.Addml.Definitions;
using Arkivverket.Arkade.Core.ExternalModels.Addml;
using Arkivverket.Arkade.Core.Logging;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Core.Tests.Base.Addml.Definitions
{
    public class AddmlDefinitionParserExceptionTest
    {
      
        [Fact]
        public void ShouldThrowExceptionWithInvalidFieldTypeInformation()
        {
            AddmlDefinitionParser parser = SetupParser("\\TestData\\fagsystem-ugyldig-addml\\fieldtype");
            AddmlDefinitionParseException ex = Assert.Throws<AddmlDefinitionParseException>(() => parser.GetAddmlDefinition());

            ex.Message.Should().Contain("No FieldType with name string-100000");
        }

        [Fact]
        public void ShouldThrowExceptionWithInvalidFlatFileTypeInformation()
        {
            AddmlDefinitionParser parser = SetupParser("\\TestData\\fagsystem-ugyldig-addml\\flatfiletype");
            AddmlDefinitionParseException ex = Assert.Throws<AddmlDefinitionParseException>(() => parser.GetAddmlDefinition());

            ex.Message.Should().Contain("No flatFileType with name fft1000");
        }
        
        private static AddmlDefinitionParser SetupParser(string archiveDirectory)
        {
            var workingDirectory =
                new WorkingDirectory(new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + archiveDirectory));
            AddmlInfo addml = AddmlUtil.ReadFromFile(workingDirectory.Root().WithFile("addml.xml").FullName);

            return new AddmlDefinitionParser(addml, workingDirectory, new StatusEventHandler());
        }
    }
}