using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Arkivverket.Arkade.Core.Util.ArchiveFormatValidation
{
    public class DiasDirectory : DiasEntry
    {
        private readonly HashSet<DiasEntry> _entries;

        public DiasDirectory(string directoryName, params DiasEntry[] diasEntries) : base(directoryName)
        {
            _entries = diasEntries.ToHashSet();
        }

        public void AddEntry(DiasEntry diasEntry)
        {
            _entries.Add(diasEntry);
        }

        public void AddEntries(params DiasEntry[] diasEntries)
        {
            foreach (DiasEntry diasEntry in diasEntries)
                AddEntry(diasEntry);
        }

        public DiasDirectory GetSubDirectory(string subDirectoryName)
        {
            return (DiasDirectory)_entries.FirstOrDefault(e => e is DiasDirectory && e.Name.Equals(subDirectoryName));
        }

        public override bool ExistsAtPath(string path)
        {
            return Directory.Exists(Path.Combine(path, Name));
        }

        public DiasEntry[] GetEntries(bool recursive = false)
        {
            var entries = new List<DiasEntry>();

            foreach (DiasEntry diasEntry in _entries)
            {
                entries.Add(diasEntry);

                if (recursive && diasEntry is DiasDirectory diasDirectory)
                    entries.AddRange(diasDirectory.GetEntries(true));
            }

            return entries.ToArray();
        }

        public List<string> GetEntryPaths(string basePath = "", bool getNonExistingOnly = false, bool recursive = false)
        {
            var entryPaths = new List<string>();

            foreach (DiasEntry diasEntry in _entries)
            {
                string entryPath = Path.Combine(basePath, diasEntry.Name);

                if (!(getNonExistingOnly && diasEntry.ExistsAtPath(basePath)))
                    entryPaths.Add(entryPath);

                if (recursive && diasEntry is DiasDirectory diasDirectory)
                    entryPaths.AddRange(diasDirectory.GetEntryPaths(entryPath, getNonExistingOnly));
            }

            return entryPaths;
        }
    }
}
