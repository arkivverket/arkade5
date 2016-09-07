using System.Diagnostics;
using Arkivverket.Arkade.Core;
using Prism.Commands;
using Prism.Mvvm;

namespace Arkivverket.Arkade.UI.ViewModels
{
    public class View000DebugViewModel : BindableBase
    {
        private readonly TestEngine _testEngine;

        public View000DebugViewModel(TestEngine testEngine)
        {
            RunTestEngineCommand = new DelegateCommand(RunTests);
            _testEngine = testEngine;
        }

        public DelegateCommand RunTestEngineCommand { get; set; }


        private void RunTests()
        {
            Debug.Print("Issued the RunTests command");
            var workingDirectory = @"C:\temp\n5-alice-liten";
            //var workingDirectory = @"C:\dev\src\arkade\src\Arkivverket.Arkade.Test\TestData\Noark5\StructureChecksums\correct";
            var archiveExtraction = new ArchiveExtraction("uuid", workingDirectory);
            archiveExtraction.ArchiveType = ArchiveType.Noark5;

            Debug.Print("Test run #1");
            _testEngine.RunTestsOnArchive(archiveExtraction);

            Debug.Print("Test run #2");
            _testEngine.RunTestsOnArchive(archiveExtraction);

            Debug.Print("Test run #3");
            _testEngine.RunTestsOnArchive(archiveExtraction);


        }
    }
}