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
        public MapState Map { get; set; }
        public PawnStates Pawns { get; set; }
        public int Tick { get; set; }
    }

    [Serializable]
    public class PawnState
    {
        public string Label { get; set; }
        public Dictionary<string, int> Position { get; set; }
        public string Equipment { get; set; }
        public Dictionary<string, float> CombatStats { get; set; }
        public Dictionary<string, float> HealthStats { get; set; }
    }


    [Serializable]
    public class PawnStates : Dictionary<string, PawnState> { }


    [Serializable]
    public class CellState : Dictionary<string, string> { }

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
