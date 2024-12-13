using System;
using System.Collections.Generic;

namespace CombatAgent
{
    public enum GameStatus
    {
        Running,
        Win,
        Lose
    }

    [Serializable]
    public class GameState
    {
        public MapState MapState { get; set; }
        public PawnStates PawnStates { get; set; }
        public int Tick { get; set; }
        public GameStatus Status { get; set; }
    }

    [Serializable]
    public class PawnState
    {
        public string Label { get; set; }
        public bool IsAlly { get; set; }
        public Dictionary<string, int> Loc { get; set; }
        public string Equipment { get; set; }
        public Dictionary<string, float> CombatStats { get; set; }
        public Dictionary<string, float> HealthStats { get; set; }
        public bool IsIncapable { get; set; }
    }


    [Serializable]
    public class PawnStates : Dictionary<string, PawnState> { }


    [Serializable]
    public class CellState {
        public Dictionary<string, int> Loc { get; set; }
        public bool IsTree { get; set; }
        public bool IsWall { get; set; }
        public bool IsPawn { get; set; }
     }

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
