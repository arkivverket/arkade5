using System.Collections.Generic;
using Arkivverket.Arkade.Core.Base.Addml;
using Arkivverket.Arkade.Core.Base.Addml.Definitions;
using Arkivverket.Arkade.Core.Tests.Base.Addml.Builders;
using FluentAssertions;
using Xunit;
using Record = Arkivverket.Arkade.Core.Base.Addml.Record;

namespace Arkivverket.Arkade.Core.Tests.Base.Addml
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