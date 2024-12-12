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
    public class PawnAction
    {
        public string Lable { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
    }

    public class GameAction
    {
        public Dictionary<string, PawnAction> PawnActions { get; set; }
    }
}
