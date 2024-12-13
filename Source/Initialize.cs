using Verse;

namespace CombatAgent
{
    [StaticConstructorOnStartup]
    public static class Start
    {
        static Start()
        {
            Log.Message("Combat Agent mod loaded successfully!");
            SocketClient.Initialize();
            Log.Message("Socket client initialized");
        }
    }
}
