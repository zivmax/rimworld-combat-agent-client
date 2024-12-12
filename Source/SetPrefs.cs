using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using UnityEngine;
using Verse;
using Verse.AI;
using Verse.AI.Group;
using Verse.Sound;
using Verse.Noise;
using Verse.Grammar;
using RimWorld;
using RimWorld.Planet;
using RimWorld.SketchGen;

namespace CombatAgent
{
    public static class PrefInitializer
    {
        public static void SetPrefs()
        {
            DebugViewSettings.neverForceNormalSpeed = true;
            Prefs.AutomaticPauseMode = AutomaticPauseMode.Never;
            Prefs.PauseOnLoad = false;
            Prefs.FullScreen = false;
            Prefs.UIScale = 1f;
            Prefs.ScreenHeight = 1080;
            Prefs.ScreenWidth = 1920;
        }
    }
}

