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

    public class PawnActions : Dictionary<string, PawnAction> { } 

    [Serializable]
    public class GameAction
    {
        public PawnActions PawnActions { get; set; }
    }
}
