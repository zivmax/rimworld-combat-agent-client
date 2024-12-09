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
        public IntVec3 Position { get; set; }
        public List<string> Apparel { get; set; }
        public string Equipment { get; set; }
        public Dictionary<string, float> CombatStats { get; set; }
        public Dictionary<string, float> HealthStats { get; set; }

        public PawnState()
        {
            Apparel = new List<string>();
            CombatStats = new Dictionary<string, float>();
            HealthStats = new Dictionary<string, float>();
        }
    }

    public class PawnStates
    {
        private readonly Dictionary<string, PawnState> pawnState = new Dictionary<string, PawnState>();

        public void Clear() => pawnState.Clear();
        public void Add(string key, PawnState value) => pawnState[key] = value;
        public bool TryGetValue(string key, out PawnState value) => pawnState.TryGetValue(key, out value);
        public Dictionary<string, PawnState> GetAll() => pawnState;

        public PawnState this[string key]
        {
            get
            {
                if (pawnState.TryGetValue(key, out var value))
                    return value;
                return null;
            }
            set
            {
                pawnState[key] = value;
            }
        }
    }

    public class CellState
    {
        public string Terrain { get; set; }
        public string Building { get; set; }
        public List<string> Items { get; set; }
        public float PathCost { get; set; }

        public CellState()
        {
            Items = new List<string>();
        }
    }

    public class MapState
    {
        private readonly CellState[,] cells;
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
