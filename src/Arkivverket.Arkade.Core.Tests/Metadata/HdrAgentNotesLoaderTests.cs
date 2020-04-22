using Arkivverket.Arkade.Core.Metadata;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Core.Tests.Metadata
{
    public class HdrAgentNotesLoaderTests
    {
        [Fact]
        public void GuessableNotesAreLoadedCorrectlyWithoutMetaNote()
        {
            string[] guessableEntityNotes =
            {
                "Sætervågen, 1000 Bø",
                "+47 99999999",
                "æøå@æøå.æøå"
            };

            var notesLoader = new HdrAgentNotesLoader(guessableEntityNotes);

            notesLoader.LoadAddress().Should().Be("Sætervågen, 1000 Bø");
            notesLoader.LoadTelephone().Should().Be("+47 99999999");
            notesLoader.LoadEmail().Should().Be("æøå@æøå.æøå");

            string[] guessableSystemNotes =
            {
                "v1.0.0",
                "Noark5",
                "v3.1"
            };

            notesLoader = new HdrAgentNotesLoader(guessableSystemNotes);

            notesLoader.LoadVersion().Should().Be("v1.0.0");
            notesLoader.LoadType().Should().Be("Noark5");
            notesLoader.LoadTypeVersion().Should().Be("v3.1");
        }

        [Fact]
        public void UnGuessableNotesAreLoadedCorrectlyWithMetaNote()
        {
            string[] unGuessableEntityNotes =
            {
                "The address",
                "The phone number",
                "The e-mail address",
                "notescontent:Address,Telephone,Email"
            };

            var notesLoader = new HdrAgentNotesLoader(unGuessableEntityNotes);

            notesLoader.LoadAddress().Should().Be("The address");
            notesLoader.LoadTelephone().Should().Be("The phone number");
            notesLoader.LoadEmail().Should().Be("The e-mail address");

            unGuessableEntityNotes = new[]
            {
                "The e-mail address",
                "The address",
                "notescontent:Email,Address"
            };

            notesLoader = new HdrAgentNotesLoader(unGuessableEntityNotes);

            notesLoader.LoadEmail().Should().Be("The e-mail address");
            notesLoader.LoadAddress().Should().Be("The address");
            notesLoader.LoadTelephone().Should().BeNull();


            string[] unGuessableSystemNotes =
            {
                "The version",
                "The type",
                "The type version",
                "notescontent:Version,Type,TypeVersion"
            };

            notesLoader = new HdrAgentNotesLoader(unGuessableSystemNotes);

            notesLoader.LoadVersion().Should().Be("The version");
            notesLoader.LoadType().Should().Be("The type");
            notesLoader.LoadTypeVersion().Should().Be("The type version");

            unGuessableSystemNotes = new[]
            {
                "The type version",
                "The version",
                "notescontent:TypeVersion,Version"
            };

            notesLoader = new HdrAgentNotesLoader(unGuessableSystemNotes);

            notesLoader.LoadTypeVersion().Should().Be("The type version");
            notesLoader.LoadVersion().Should().Be("The version");
            notesLoader.LoadType().Should().BeNull();
        }

        [Fact]
        public void InvalidMetaNoteShouldBeIgnoredAndCauseFallbackOnContentExamining()
        {
            string[] notesContainingMetaNoteWithMissingField =
            {
                "The address",
                "The phone number", // Not loader guessable
                "The e-mail address",
                "notescontent:Address,Telephone"
            };

            string[] notesContainingMetaNoteWithExcessiveField =
            {
                "The address",
                "The phone number", // Not loader guessable
                "notescontent:Address,Telephone,Email"
            };

            string[] notesContainingMetaNoteWithInvalidFields =
            {
                "The address",
                "The phone number",
                "The e-mail address",
                "notescontent:Home Address,telephone,E-mail"
            };

            // LoadTelephone() should return null as the invalid meta-notes given are ignored and the
            // fallback to content examination doesn't recognize "The phone number" as a phone number

            new HdrAgentNotesLoader(notesContainingMetaNoteWithMissingField).LoadTelephone().Should().BeNull();
            new HdrAgentNotesLoader(notesContainingMetaNoteWithExcessiveField).LoadTelephone().Should().BeNull();
            new HdrAgentNotesLoader(notesContainingMetaNoteWithInvalidFields).LoadTelephone().Should().BeNull();
        }
    }
}
