using Arkivverket.Arkade.Core.Metadata;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Core.Tests.Metadata
{
    public class HdrAgentNotesWriterTests
    {
        [Fact]
        public void GetNotesTest()
        {
            var notesWriter = new HdrAgentNotesWriter();
            notesWriter.AddAddress("Some address");
            notesWriter.AddTelephone("Some telephone");
            notesWriter.AddEmail("Some email");
            string[] notes = notesWriter.GetNotes();
            notes[0].Should().Be("Some address");
            notes[1].Should().Be("Some telephone");
            notes[2].Should().Be("Some email");
            notes[3].Should().Be("notescontent:Address,Telephone,Email");

            notesWriter = new HdrAgentNotesWriter();
            notesWriter.AddTelephone("Some telephone");
            notesWriter.AddEmail("Some email");
            notes = notesWriter.GetNotes();
            notes[0].Should().Be("Some telephone");
            notes[1].Should().Be("Some email");
            notes[2].Should().Be("notescontent:Telephone,Email");

            notesWriter = new HdrAgentNotesWriter();
            notesWriter.AddAddress("Some address");
            notesWriter.AddEmail("Some email");
            notes = notesWriter.GetNotes();
            notes[0].Should().Be("Some address");
            notes[1].Should().Be("Some email");
            notes[2].Should().Be("notescontent:Address,Email");

            notesWriter = new HdrAgentNotesWriter();
            notesWriter.AddAddress("Some address");
            notesWriter.AddTelephone("Some telephone");
            notes = notesWriter.GetNotes();
            notes[0].Should().Be("Some address");
            notes[1].Should().Be("Some telephone");
            notes[2].Should().Be("notescontent:Address,Telephone");

            notesWriter = new HdrAgentNotesWriter();
            notesWriter.AddAddress("Some address");
            notes = notesWriter.GetNotes();
            notes[0].Should().Be("Some address");
            notes[1].Should().Be("notescontent:Address");

            notesWriter = new HdrAgentNotesWriter();
            notesWriter.AddTelephone("Some telephone");
            notes = notesWriter.GetNotes();
            notes[0].Should().Be("Some telephone");
            notes[1].Should().Be("notescontent:Telephone");

            notesWriter = new HdrAgentNotesWriter();
            notesWriter.AddEmail("Some email");
            notes = notesWriter.GetNotes();
            notes[0].Should().Be("Some email");
            notes[1].Should().Be("notescontent:Email");

            notesWriter = new HdrAgentNotesWriter();
            notesWriter.AddVersion("Some version");
            notesWriter.AddType("Some type");
            notesWriter.AddTypeVersion("Some type-version");
            notes = notesWriter.GetNotes();
            notes[0].Should().Be("Some version");
            notes[1].Should().Be("Some type");
            notes[2].Should().Be("Some type-version");
            notes[3].Should().Be("notescontent:Version,Type,TypeVersion");

            notesWriter = new HdrAgentNotesWriter();
            notesWriter.AddType("Some type");
            notesWriter.AddTypeVersion("Some type-version");
            notes = notesWriter.GetNotes();
            notes[0].Should().Be("Some type");
            notes[1].Should().Be("Some type-version");
            notes[2].Should().Be("notescontent:Type,TypeVersion");

            notesWriter = new HdrAgentNotesWriter();
            notesWriter.AddVersion("Some version");
            notesWriter.AddTypeVersion("Some type-version");
            notes = notesWriter.GetNotes();
            notes[0].Should().Be("Some version");
            notes[1].Should().Be("Some type-version");
            notes[2].Should().Be("notescontent:Version,TypeVersion");

            notesWriter = new HdrAgentNotesWriter();
            notesWriter.AddVersion("Some version");
            notesWriter.AddType("Some type");
            notes = notesWriter.GetNotes();
            notes[0].Should().Be("Some version");
            notes[1].Should().Be("Some type");
            notes[2].Should().Be("notescontent:Version,Type");

            notesWriter = new HdrAgentNotesWriter();
            notesWriter.AddVersion("Some version");
            notes = notesWriter.GetNotes();
            notes[0].Should().Be("Some version");
            notes[1].Should().Be("notescontent:Version");

            notesWriter = new HdrAgentNotesWriter();
            notesWriter.AddType("Some type");
            notes = notesWriter.GetNotes();
            notes[0].Should().Be("Some type");
            notes[1].Should().Be("notescontent:Type");

            notesWriter = new HdrAgentNotesWriter();
            notesWriter.AddTypeVersion("Some type-version");
            notes = notesWriter.GetNotes();
            notes[0].Should().Be("Some type-version");
            notes[1].Should().Be("notescontent:TypeVersion");

            notesWriter = new HdrAgentNotesWriter();
            notesWriter.HasNotes().Should().BeFalse();
            notesWriter.GetNotes().Should().BeEmpty();
        }
    }
}
