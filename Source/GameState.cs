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

namespace CombatAgent
{
    [Serializable]
    public class GameState
    {
        public MapState MapState { get; set; }
        public PawnStates PawnStates { get; set; }
        public int Tick { get; set; }
    }

    [Serializable]
    public class PawnState
    {
        public string Label { get; set; }
        public Dictionary<string, int> Loc { get; set; }
        public string Equipment { get; set; }
        public Dictionary<string, float> CombatStats { get; set; }
        public Dictionary<string, float> HealthStats { get; set; }
        public bool IsIncapable { get; set; }
    }


    [Serializable]
    public class PawnStates : Dictionary<string, PawnState> { }


    [Serializable]
    public class CellState : Dictionary<string, bool> { }

    [Serializable]
    public class MapState
    {
        public Dictionary<string, CellState> Cells { get; set; }
        public int Width { get; private set; }
        public int Height { get; private set; }

        public MapState(int width, int height)
        {
            Width = width;
            Height = height;
            Cells = new Dictionary<string, CellState>();
        }
    }
}
