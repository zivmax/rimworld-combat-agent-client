using System;
using System.Collections.Generic;

namespace CombatAgent
{
    [Serializable]
    public class PawnAction
    {
        public string Label { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
    }

    [Serializable]
    public class GameAction
    {
        public Dictionary<string, PawnAction> PawnActions { get; set; }
    }
}
