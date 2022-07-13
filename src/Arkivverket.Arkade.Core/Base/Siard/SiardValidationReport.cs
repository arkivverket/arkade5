using System.Collections.Generic;

namespace Arkivverket.Arkade.Core.Base.Siard
{
    public class SiardValidationReport
    {
        public List<string> Results { get; }
        public List<string> Errors { get; }
        public ArchiveTestingTool TestingTool { get; set; }

        public SiardValidationReport(List<string> results, List<string> errors)
        {
            Results = results;
            Errors = errors;
        }
    }
}
