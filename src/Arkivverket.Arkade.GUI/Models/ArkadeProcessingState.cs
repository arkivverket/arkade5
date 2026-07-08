namespace Arkivverket.Arkade.GUI.Models
{
    public static class ArkadeProcessingState
    {
        public static bool LoadingIsStarted { get; set; }
        public static bool TestingIsStarted { get; set; }
        public static bool PackingIsStarted { get; set; }
        public static bool PackingIsFinished { get; set; }

        public static void Reset()
        {
            LoadingIsStarted = false;
            TestingIsStarted = false;
            PackingIsStarted = false;
            PackingIsFinished = false;
        }
    }
}
