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
    public class GameState
    {
        public MapState Map { get; set; }
        public PawnStates Pawns { get; set; }
        public int Tick { get; set; }

        public GameState()
        {
            Pawns = new PawnStates();
        }
    }

    public class PawnState
    {
        public string Label { get; set; }
        public Tuple<int, int> Position { get; set; }
        public string Equipment { get; set; }
        public Dictionary<string, float> CombatStats { get; set; }
        public Dictionary<string, float> HealthStats { get; set; }
    }


    public class PawnStates : Dictionary<string, PawnState> { }


    public class CellState
    {
        public string Terrain { get; set; }
        public string Building { get; set; }
        public float PathCost { get; set; }

        public CellState()
        {
            Terrain = "";
            Building = "";
            PathCost = 0;
        }
    }

    public class MapState
    {
        public CellState[,] cells = null;
        public int Width { get; private set; }
        public int Height { get; private set; }

        public MapState(int width, int height)
        {
            Width = width;
            Height = height;
            cells = new CellState[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    cells[x, y] = new CellState();
                }
            }
        }

        public CellState this[int x, int y]
        {
            get
            {
                if (x < 0 || x >= Width || y < 0 || y >= Height)
                    return null;
                return cells[x, y];
            }
            set
            {
                if (x >= 0 && x < Width && y >= 0 && y < Height)
                    cells[x, y] = value;
            }
        }
    }
}
