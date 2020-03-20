using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Arkivverket.Arkade.Core.Metadata
{
    public static class HdrAgentNotesLoader
    {
        public static string GetAddress(IEnumerable<string> notes)
        {
            return notes.FirstOrDefault(LooksLikeAddress);
        }

        public static string GetTelephone(IEnumerable<string> notes)
        {
            return notes.FirstOrDefault(LooksLikePhoneNumber);
        }

        public static string GetEmail(IEnumerable<string> notes)
        {
            return notes.FirstOrDefault(LooksLikeEmailAddress);
        }

        public static string GetVersion(IEnumerable<string> notes)
        {
            notes = notes.ToList();

            string type = GetType(notes);

            // Find first occurrence of a version number defined before Type. That's probably the version ...
            return notes.TakeWhile(n => !n.Equals(type)).ToArray().FirstOrDefault(LooksLikeSystemVersion);
        }

        public static string GetType(IEnumerable<string> notes)
        {
            return notes.FirstOrDefault(LooksLikeSystemType);
        }

        public static string GetTypeVersion(IEnumerable<string> notes)
        {
            notes = notes.ToList();

            string type = GetType(notes);

            // Find first occurrence of a version number defined after Type. That's probably the type-version ...
            return notes.SkipWhile(n => !n.Equals(type)).ToArray().FirstOrDefault(LooksLikeSystemTypeVersion);
        }

        private static bool LooksLikeAddress(string possibleAddress)
        {
            return !LooksLikeEmailAddress(possibleAddress) && !LooksLikePhoneNumber(possibleAddress);
        }

        private static bool LooksLikeEmailAddress(string possibleEmailAddress)
        {
            return Regex.IsMatch(possibleEmailAddress,
                @"\A(?:[æøåa-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[æøåa-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[æøåa-z0-9](?:[æøåa-z0-9-]*[æøåa-z0-9])?\.)+[æøåa-z0-9](?:[æøåa-z0-9-]*[æøåa-z0-9])?)\Z",
                RegexOptions.IgnoreCase
            );
        }

        private static bool LooksLikePhoneNumber(string possiblePhoneNumber)
        {
            return Regex.IsMatch(possiblePhoneNumber, @"^\+?(\d[\d-. ]+)?(\([\d-. ]+\))?[\d-. ]+\d$");
        }

        private static bool LooksLikeSystemVersion(string possibleSystemVersion)
        {
            return LooksLikeVersionNumber(possibleSystemVersion);
        }

        private static bool LooksLikeSystemType(string possibleSystemType)
        {
            return MetsTranslationHelper.IsValidSystemType(possibleSystemType);
        }

        private static bool LooksLikeSystemTypeVersion(string possibleSystemTypeVersion)
        {
            return LooksLikeVersionNumber(possibleSystemTypeVersion);
        }

        private static bool LooksLikeVersionNumber(string possibleVersionNumber)
        {
            return Regex.IsMatch(possibleVersionNumber, @"\d+(\.\d+)+");
        }
    }
}
