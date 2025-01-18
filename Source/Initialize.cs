using Verse;
using UnityEngine;


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
            Config.GetConfigFromCLIArgs();
            PrefInitializer.SetPrefs();
            Config.LogConfig();
            if (Config.Headless)
            { Debug.unityLogger.logHandler = new SuppressLogHandler(); }
        }
    }
}
