using Arkivverket.Arkade.Core.Addml;

namespace Arkivverket.Arkade.Core
{
    public class AddmlDatasetTestEngine
    {
        private readonly AddmlDefinition _addmlDefinition;

        public AddmlDatasetTestEngine(AddmlDefinition addmlDefinition)
        {
            _addmlDefinition = addmlDefinition;
        }

        public TestSuite RunTests()
        {
            var flatFiles = _addmlDefinition.GetFlatFiles();


            return new TestSuite();
        }
    }
}