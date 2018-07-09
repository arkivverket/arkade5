using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Base.Addml;
using Arkivverket.Arkade.Core.Base.Addml.Definitions;
using Arkivverket.Arkade.Core.Base.Addml.Definitions.DataTypes;
using Arkivverket.Arkade.Core.Logging;
using Arkivverket.Arkade.Core.Util;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Test.Base.Addml
{
    public class Jegerreg98Test
    {
        [Fact]
        public void ShouldParseJegerreg98ArkivuttrekkXml()
        {
            var externalContentDirectory = new DirectoryInfo(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\TestData\\jegerreg-98-dos");
            var workingDirectory = new WorkingDirectory(ArkadeProcessingArea.WorkDirectory, externalContentDirectory);
            AddmlInfo addml = AddmlUtil.ReadFromFile(workingDirectory.Content().WithFile("arkivuttrekk.xml").FullName);

            AddmlDefinition addmlDefinition = new AddmlDefinitionParser(addml, workingDirectory, new StatusEventHandler()).GetAddmlDefinition();

            List<AddmlFlatFileDefinition> addmlFlatFileDefinitions = addmlDefinition.AddmlFlatFileDefinitions;
            addmlFlatFileDefinitions.Count.Should().Be(10);
            {
                addmlFlatFileDefinitions[0].Name.Should().Be("ffd_3");
                addmlFlatFileDefinitions[0].FileName.Should().Be("jeger.dat");
                addmlFlatFileDefinitions[0].Encoding.Should().Be(Encodings.ISO_8859_1);
                addmlFlatFileDefinitions[0].RecordSeparator.Should().Be(Separator.CRLF);
                addmlFlatFileDefinitions[0].AddmlRecordDefinitions.Count.Should().Be(1);
                AddmlRecordDefinition addmlRecordDefinition = addmlFlatFileDefinitions[0].AddmlRecordDefinitions[0];
                List<AddmlFieldDefinition> addmlFieldDefinitions = addmlRecordDefinition.AddmlFieldDefinitions;
                addmlFieldDefinitions.Count.Should().Be(36);
                addmlFieldDefinitions[0].Name.Should().Be("fodselsnummer");
                addmlRecordDefinition.PrimaryKey.Should()
                    .Equal(new List<AddmlFieldDefinition>() {addmlFieldDefinitions[0]});
            }
            {
                addmlFlatFileDefinitions[1].Name.Should().Be("ffd_4");
                addmlFlatFileDefinitions[1].FileName.Should().Be("ut_jeger.dat");
                addmlFlatFileDefinitions[1].Encoding.Should().Be(Encodings.ISO_8859_1);
                addmlFlatFileDefinitions[1].RecordSeparator.Should().Be(Separator.CRLF);
                addmlFlatFileDefinitions[1].AddmlRecordDefinitions.Count.Should().Be(1);
                AddmlRecordDefinition addmlRecordDefinition = addmlFlatFileDefinitions[1].AddmlRecordDefinitions[0];
                List<AddmlFieldDefinition> addmlFieldDefinitions = addmlRecordDefinition.AddmlFieldDefinitions;
                addmlFieldDefinitions.Count.Should().Be(37);
                addmlFieldDefinitions[0].Name.Should().Be("plassnummer");
                addmlFieldDefinitions[0].Type.Should().Be(IntegerDataType.Default);
                addmlFieldDefinitions[1].Name.Should().Be("landkort");
                addmlFieldDefinitions[8].Name.Should().Be("personnummer");
                addmlRecordDefinition.PrimaryKey.Should()
                    .Equal(new List<AddmlFieldDefinition>()
                    {
                        addmlFieldDefinitions[0],
                        addmlFieldDefinitions[1],
                        addmlFieldDefinitions[8]
                    });
            }
            {
                addmlFlatFileDefinitions[2].Name.Should().Be("ffd_5");
                addmlFlatFileDefinitions[2].FileName.Should().Be("ikkejeg.dat");
                addmlFlatFileDefinitions[2].Encoding.Should().Be(Encodings.ISO_8859_1);
                addmlFlatFileDefinitions[2].RecordSeparator.Should().Be(Separator.CRLF);
                addmlFlatFileDefinitions[2].AddmlRecordDefinitions.Count.Should().Be(1);
                AddmlRecordDefinition addmlRecordDefinition = addmlFlatFileDefinitions[2].AddmlRecordDefinitions[0];
                List<AddmlFieldDefinition> addmlFieldDefinitions = addmlRecordDefinition.AddmlFieldDefinitions;
                addmlFieldDefinitions.Count.Should().Be(44);
                addmlFieldDefinitions[0].Name.Should().Be("fodselsnummer");
                addmlFieldDefinitions[0].StartPosition.Should().Be(1);
                addmlFieldDefinitions[0].FixedLength.Should().Be(11);
                addmlFieldDefinitions[0].IsUnique.Should().BeTrue();
                addmlFieldDefinitions[0].IsNullable.Should().BeFalse();
                addmlFieldDefinitions[0].MaxLength.Should().NotHaveValue();
                addmlFieldDefinitions[0].MinLength.Should().NotHaveValue();
                addmlFieldDefinitions[0].Type.Should().Be(StringDataType.Default);
  /*              addmlFieldDefinitions[0].ForeignKeys.First().Should().Be(
                    addmlFlatFileDefinitions[0].AddmlRecordDefinitions[0].AddmlFieldDefinitions[0]
                );*/

                addmlFieldDefinitions[1].Name.Should().Be("etternavn");
                addmlFieldDefinitions[1].StartPosition.Should().Be(12);
                addmlFieldDefinitions[1].FixedLength.Should().Be(25);
                addmlFieldDefinitions[1].IsUnique.Should().BeFalse();
                addmlFieldDefinitions[1].IsNullable.Should().BeTrue();
                addmlFieldDefinitions[1].MaxLength.Should().NotHaveValue();
                addmlFieldDefinitions[1].MinLength.Should().NotHaveValue();
                addmlFieldDefinitions[1].Type.Should().Be(StringDataType.Default);
//                addmlFieldDefinitions[1].ForeignKeys.Should().BeEmpty();

                addmlRecordDefinition.PrimaryKey.Should()
                    .Equal(new List<AddmlFieldDefinition>()
                    {
                        addmlFieldDefinitions[0]
                    });


                TestDateDataType(addmlDefinition);
                TestBooleanDataType(addmlDefinition);
                TestFloatDataType(addmlDefinition);
            }
        }

        private void TestDateDataType(AddmlDefinition addmlDefinition)
        {
            AddmlFlatFileDefinition addmlFlatFileDefinition = addmlDefinition.AddmlFlatFileDefinitions[2];
            addmlFlatFileDefinition.Name.Should().Be("ffd_5");
            addmlFlatFileDefinition.FileName.Should().Be("ikkejeg.dat");
            AddmlFieldDefinition addmlFieldDefinition = addmlFlatFileDefinition.AddmlRecordDefinitions[0].AddmlFieldDefinitions[35];
            addmlFieldDefinition.Name.Should().Be("fradato");
            addmlFieldDefinition.Type.Should().Be(new DateDataType("ddmmyyyy"));
        }

        private void TestFloatDataType(AddmlDefinition addmlDefinition)
        {
            AddmlFlatFileDefinition addmlFlatFileDefinition = addmlDefinition.AddmlFlatFileDefinitions[5];
            addmlFlatFileDefinition.Name.Should().Be("ffd_8");
            addmlFlatFileDefinition.FileName.Should().Be("betalt.dat");
            AddmlFieldDefinition addmlFieldDefinition = addmlFlatFileDefinition.AddmlRecordDefinitions[0].AddmlFieldDefinitions[8];
            addmlFieldDefinition.Name.Should().Be("beløp");
            addmlFieldDefinition.Type.Should().Be(new FloatDataType());
        }

        private void TestBooleanDataType(AddmlDefinition addmlDefinition)
        {
            AddmlFlatFileDefinition addmlFlatFileDefinition = addmlDefinition.AddmlFlatFileDefinitions[3];
            addmlFlatFileDefinition.Name.Should().Be("ffd_6");
            addmlFlatFileDefinition.FileName.Should().Be("jegerK.dat");
            AddmlFieldDefinition addmlFieldDefinition = addmlFlatFileDefinition.AddmlRecordDefinitions[0].AddmlFieldDefinitions[10];
            addmlFieldDefinition.Name.Should().Be("avgift");
            addmlFieldDefinition.Type.Should().Be(new BooleanDataType("J/N"));
        }

    }
}