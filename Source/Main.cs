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
    public class CombatAgentMain : GameComponent
    {
        public CombatAgentMain(Game game) { }
        public override void GameComponentTick()
        {
            if (Find.TickManager.TicksGame % 500 == 0)
            {
                StateCollector.CollectPawnData();
                StateCollector.CollectMapData();
                var state = new GameState
                {
                    Map = StateCollector.GetMapState(),
                    Pawns = StateCollector.GetPawnStates(),
                    Tick = Find.TickManager.TicksGame
                };
                StateCollector.LogMapData();
                StateCollector.LogPawnData();
                SocketClient.SendGameState(state);
            }
        }
    }
}
