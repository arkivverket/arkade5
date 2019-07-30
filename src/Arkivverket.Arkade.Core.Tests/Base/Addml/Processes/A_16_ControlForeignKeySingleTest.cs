using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Base.Addml;
using Arkivverket.Arkade.Core.Base.Addml.Definitions;
using Arkivverket.Arkade.Core.Base.Addml.Processes;
using Arkivverket.Arkade.Core.Base.Addml.Processes.Internal;
using Arkivverket.Arkade.Core.Tests.Base.Addml.Builders;
using FluentAssertions;
using Xunit;
using Record = Arkivverket.Arkade.Core.Base.Addml.Record;

namespace Arkivverket.Arkade.Core.Tests.Base.Addml.Processes
{
    public class A_16_ControlForeignKeySingleTest
    {
        public A_16_ControlForeignKeySingleTest()
        {
            AddmlFlatFileDefinition attachmentFileDefinition = new AddmlFlatFileDefinitionBuilder().Build();
            _attachmentRecordDef = new AddmlRecordDefinitionBuilder()
                .WithAddmlFlatFileDefinition(attachmentFileDefinition)
                .Build();
            _messageIdPrimaryKeyFieldDef = new AddmlFieldDefinitionBuilder()
                .WithRecordDefinition(_attachmentRecordDef)
                .WithName("MessageId")
                .IsPartOfPrimaryKey(true)
                .Build();

            AddmlForeignKey foreignKey = new AddmlForeignKey("fk1");

            AddmlFlatFileDefinition addmlFlatFileDefinition = new AddmlFlatFileDefinitionBuilder().Build();
            _recordDef = new AddmlRecordDefinitionBuilder()
                .WithAddmlFlatFileDefinition(addmlFlatFileDefinition)
                .WithRecordProcess(A_16_ControlForeignKey.Name)
                .WithForeignKey(foreignKey)
                .Build();

            _primaryKeyFieldDef = new AddmlFieldDefinitionBuilder()
                .WithRecordDefinition(_recordDef)
                .WithName("Id")
                .IsPartOfPrimaryKey(true)
                .Build();
            _foreignKeyMessageIdFieldDef = new AddmlFieldDefinitionBuilder()
                .WithRecordDefinition(_recordDef)
                .WithName("MessageId")
                .Build();

            foreignKey.ForeignKeys.Add(_foreignKeyMessageIdFieldDef);
            foreignKey.ForeignKeyReferenceIndexes.Add(_messageIdPrimaryKeyFieldDef.GetIndex());
            foreignKey.ForeignKeyReferenceFields.Add(_messageIdPrimaryKeyFieldDef);
        }

        private readonly AddmlRecordDefinition _attachmentRecordDef;
        private readonly AddmlFieldDefinition _messageIdPrimaryKeyFieldDef;

        private readonly AddmlRecordDefinition _recordDef;
        private readonly AddmlFieldDefinition _primaryKeyFieldDef;
        private readonly AddmlFieldDefinition _foreignKeyMessageIdFieldDef;


        [Fact]
        public void ShouldReturnErrorWhenSingleForeignKeyReferenceDoesNotExist()
        {
            var attachmentDataRecord1 = new Record(_attachmentRecordDef, new Field(_messageIdPrimaryKeyFieldDef, "556677"));
            var attachmentDataRecord2 = new Record(_attachmentRecordDef, new Field(_messageIdPrimaryKeyFieldDef, "1002"));

            var testRecord1 = new Record(_recordDef, 
                new Field(_primaryKeyFieldDef, "1234"), 
                new Field(_foreignKeyMessageIdFieldDef, "1001"));

            var testRecord2 = new Record(_recordDef,
                new Field(_primaryKeyFieldDef, "1235"),
                new Field(_foreignKeyMessageIdFieldDef, "1002"));

            TestRun testRun = RunTest(testRecord1, testRecord2, attachmentDataRecord1, attachmentDataRecord2);
            testRun.IsSuccess().Should().BeFalse();
        }

        [Fact]
        public void ShouldReturnSuccessWhenSingleForeignKeyReferenceExist()
        {
            var attachmentDataRecord = new Record(_attachmentRecordDef, 
                new Field(_messageIdPrimaryKeyFieldDef, "1001"));

            var testRecord = new Record(_recordDef, 
                new Field(_primaryKeyFieldDef, "1234"),
                new Field(_foreignKeyMessageIdFieldDef, "1001"));

            TestRun testRun = RunTest(testRecord, attachmentDataRecord);
            testRun.IsSuccess().Should().BeTrue();
        }

        [Fact]
        public void ShouldReturnSuccessWhenAllSingleForeignKeyReferenceExist()
        {
            var attachmentDataRecord1 = new Record(_attachmentRecordDef, new Field(_messageIdPrimaryKeyFieldDef, "1001"));
            var attachmentDataRecord2 = new Record(_attachmentRecordDef, new Field(_messageIdPrimaryKeyFieldDef, "1002"));

            var testRecord1 = new Record(_recordDef,
                new Field(_primaryKeyFieldDef, "1234"),
                new Field(_foreignKeyMessageIdFieldDef, "1001"));

            var testRecord2 = new Record(_recordDef,
                new Field(_primaryKeyFieldDef, "1235"),
                new Field(_foreignKeyMessageIdFieldDef, "1002"));

            TestRun testRun = RunTest(testRecord1, testRecord2, attachmentDataRecord1, attachmentDataRecord2);
            testRun.IsSuccess().Should().BeTrue();
        }


        public static TestRun RunTest(params Record[] records)
        {
            var collectPrimarykeyProcess = new AI_01_CollectPrimaryKey();
            var controlForeignKeyProcess = new A_16_ControlForeignKey();

            foreach (var record in records)
            {
                collectPrimarykeyProcess.Run(record);
                controlForeignKeyProcess.Run(record);
            }

            // need to do transfering of primary keys manually in the test environment
            controlForeignKeyProcess.CollectedPrimaryKeys = collectPrimarykeyProcess._primaryKeys;

            return controlForeignKeyProcess.GetTestRun();
        }
    }
}