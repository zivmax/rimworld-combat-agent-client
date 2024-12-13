using Verse;

namespace CombatAgent
{
    public static class PrefInitializer
    {
        public static void SetPrefs()
        {
            Prefs.DevMode = true;
            DebugViewSettings.neverForceNormalSpeed = true;
            Prefs.AutomaticPauseMode = AutomaticPauseMode.Never;
            Prefs.PauseOnLoad = false;
            Prefs.FullScreen = false;
            Prefs.UIScale = 1.5f;
            Prefs.ScreenHeight = 1080;
            Prefs.ScreenWidth = 1920;
        }
    }
}

