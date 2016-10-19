using System.Collections.Generic;
using System.Text;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Core.Addml;
using Arkivverket.Arkade.Core.Addml.Definitions;
using FluentAssertions;
using FluentAssertions.Types;
using Xunit;

namespace Arkivverket.Arkade.Test.Core.Addml.Definitions
{
    public class Jegerreg98Test
    {
        [Fact]
        public void ShouldParseJegerreg98ArkivuttrekkXml()
        {
            AddmlInfo addml = AddmlUtil.ReadFromBaseDirectory("..\\..\\TestData\\jegerreg-98-dos\\arkivuttrekk.xml");

            AddmlDefinitionParser parser = new AddmlDefinitionParser(addml);

            AddmlDefinition addmlDefinition = parser.GetAddmlDefinition();
            List<AddmlFlatFileDefinition> addmlFlatFileDefinitions = addmlDefinition.AddmlFlatFileDefinitions;
            addmlFlatFileDefinitions.Count.Should().Be(10);
            {
                addmlFlatFileDefinitions[0].Name.Should().Be("ffd_3");
                addmlFlatFileDefinitions[0].FileName.Should().Be("jeger.dat");
                addmlFlatFileDefinitions[0].Encoding.Should().Be(Encodings.ISO_8859_1);
                addmlFlatFileDefinitions[0].RecordSeparator.Should().Be(RecordSeparator.CRLF);
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
                addmlFlatFileDefinitions[1].RecordSeparator.Should().Be(RecordSeparator.CRLF);
                addmlFlatFileDefinitions[1].AddmlRecordDefinitions.Count.Should().Be(1);
                AddmlRecordDefinition addmlRecordDefinition = addmlFlatFileDefinitions[1].AddmlRecordDefinitions[0];
                List<AddmlFieldDefinition> addmlFieldDefinitions = addmlRecordDefinition.AddmlFieldDefinitions;
                addmlFieldDefinitions.Count.Should().Be(37);
                addmlFieldDefinitions[0].Name.Should().Be("plassnummer");
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
                addmlFlatFileDefinitions[2].RecordSeparator.Should().Be(RecordSeparator.CRLF);
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
                //addmlFieldDefinitions[0].Type.Should().Be(Types.String);
                addmlFieldDefinitions[0].ForeignKey.Should().Be(
                    addmlFlatFileDefinitions[0].AddmlRecordDefinitions[0].AddmlFieldDefinitions[0]
                );

                addmlFieldDefinitions[1].Name.Should().Be("etternavn");
                addmlFieldDefinitions[1].StartPosition.Should().Be(12);
                addmlFieldDefinitions[1].FixedLength.Should().Be(25);
                addmlFieldDefinitions[1].IsUnique.Should().BeFalse();
                addmlFieldDefinitions[1].IsNullable.Should().BeTrue();
                addmlFieldDefinitions[1].MaxLength.Should().NotHaveValue();
                addmlFieldDefinitions[1].MinLength.Should().NotHaveValue();
                //addmlFieldDefinitions[1].Type.Should().Be(Types.String);
                addmlFieldDefinitions[1].ForeignKey.Should().BeNull();


                addmlRecordDefinition.PrimaryKey.Should()
                    .Equal(new List<AddmlFieldDefinition>()
                    {
                        addmlFieldDefinitions[0]
                    });


            }
        }
    }
}