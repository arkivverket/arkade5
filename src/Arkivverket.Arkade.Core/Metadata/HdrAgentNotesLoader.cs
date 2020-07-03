using System;
using System.Linq;
using System.Text.RegularExpressions;
using Arkivverket.Arkade.Core.Util;
using Serilog;

namespace Arkivverket.Arkade.Core.Metadata
{
    public class HdrAgentNotesLoader
    {
        private readonly string[] _contentNotes;
        private readonly string[] _validMetaNoteFields;

        private enum HdrAgentNoteField
        {
            Address,
            Telephone,
            Email,
            Version,
            Type,
            TypeVersion
        }

        private readonly ILogger _logger = Log.ForContext<HdrAgentNotesLoader>();

        public HdrAgentNotesLoader(string[] notes)
        {
            string metaNote = notes.FirstOrDefault(n => n.StartsWith(ArkadeConstants.MetsHdrAgentMetaNoteKeyWord));

            if (metaNote != null)
            {
                _contentNotes = notes.Where(n => n != metaNote).ToArray();

                _validMetaNoteFields = GetValidMetaNoteFields(metaNote);
            }
            else
            {
                _contentNotes = notes;
            }
        }

        public string LoadAddress()
        {
            return Load(HdrAgentNoteField.Address);
        }

        public string LoadTelephone()
        {
            return Load(HdrAgentNoteField.Telephone);
        }

        public string LoadEmail()
        {
            return Load(HdrAgentNoteField.Email);
        }

        public string LoadVersion()
        {
            return Load(HdrAgentNoteField.Version);
        }

        public string LoadType()
        {
            return Load(HdrAgentNoteField.Type);
        }

        public string LoadTypeVersion()
        {
            return Load(HdrAgentNoteField.TypeVersion);
        }

        private string Load(HdrAgentNoteField field)
        {
            if (_validMetaNoteFields != null)
            {
                return LoadByMetaNote(field);
            }

            _logger.Warning("No valid meta-note was found. Arkade will try to find something that looks"
                            + " like " + field + "amongst the notes for the current mets hdrAgent");

            return LoadByContentExamination(field);
        }

        private string[] GetValidMetaNoteFields(string metaNote)
        {
            string[] metaNoteFields = metaNote.Substring(ArkadeConstants.MetsHdrAgentMetaNoteKeyWord.Length).Split(',');

            if (metaNoteFields.Any(field => !Enum.IsDefined(typeof(HdrAgentNoteField), field)))
            {
                _logger.Error($"Meta-note \"{metaNote}\" contains unrecognized fields. Meta-note will be ignored.");

                return null;
            }

            if (metaNoteFields.Length != _contentNotes.Length)
            {
                _logger.Error($"Number of fields in meta-note \"{metaNote}\" doesn't match the number"
                              + "of notes for the metsHdr agent. Meta-note will be ignored.");

                return null;
            }

            return metaNoteFields;
        }

        private string LoadByMetaNote(HdrAgentNoteField field)
        {
            int fieldPosition = Array.IndexOf(_validMetaNoteFields, field.ToString());

            return _contentNotes.ElementAtOrDefault(fieldPosition);
        }

        private string LoadByContentExamination(HdrAgentNoteField field)
        {
            string type = _contentNotes.FirstOrDefault(LooksLikeSystemType);

            switch (field)
            {
                case HdrAgentNoteField.Address:
                    return _contentNotes.FirstOrDefault(LooksLikeAddress);
                case HdrAgentNoteField.Telephone:
                    return _contentNotes.FirstOrDefault(LooksLikePhoneNumber);
                case HdrAgentNoteField.Email:
                    return _contentNotes.FirstOrDefault(LooksLikeEmailAddress);
                case HdrAgentNoteField.Version:
                    string[] notesDefinedBeforeType = _contentNotes.TakeWhile(n => !n.Equals(type)).ToArray();
                    return notesDefinedBeforeType.FirstOrDefault(LooksLikeSystemVersion);
                case HdrAgentNoteField.Type:
                    return _contentNotes.FirstOrDefault(LooksLikeSystemType);
                case HdrAgentNoteField.TypeVersion:
                    string[] notesDefinedAfterType = _contentNotes.SkipWhile(n => !n.Equals(type)).ToArray();
                    return notesDefinedAfterType.FirstOrDefault(LooksLikeSystemTypeVersion);
                default:
                    return null;
            }
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
