using System.Collections.Generic;
using Arkivverket.Arkade.Core.Addml;
using Arkivverket.Arkade.Core.Addml.Definitions;
using Arkivverket.Arkade.Test.Core.Addml.Builders;
using FluentAssertions;
using Xunit;
using Record = Arkivverket.Arkade.Core.Addml.Record;

namespace Arkivverket.Arkade.Test.Core.Addml
{
    public class RecordTest
    {
        [Fact]
        public void RecordValueShouldContainFieldSeparator()
        {
            AddmlRecordDefinition recordDefinition = new AddmlRecordDefinitionBuilder()
                .WithAddmlFlatFileDefinition(new AddmlFlatFileDefinitionBuilder()
                    .WithFieldSeparator("AA")
                    .Build())
                .Build();
            List<Field> fields = new List<Field>
            {
                new Field(null, "1"),
                new Field(null, "2"),
                new Field(null, "3")
            };

            Record record = new Record(recordDefinition, fields);

            record.Value.Should().Be("1AA2AA3");
        }
    }
}