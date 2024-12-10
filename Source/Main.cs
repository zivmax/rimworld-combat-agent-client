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
    public class CombatAgentMain : GameComponent
    {
        public CombatAgentMain(Game game) { }
        public override void GameComponentTick()
        {
            if (Find.TickManager.TicksGame % 500 == 0)
            {
                StateCollector.CollectPawnState();
                StateCollector.CollectMapState();
                var state = new GameState
                {
                    Map = StateCollector.GetMapState(),
                    Pawns = StateCollector.GetPawnStates(),
                    Tick = Find.TickManager.TicksGame
                };
                StateCollector.LogCellState(IntVec3.Zero);
                SocketClient.SendGameState(state);
            }
        }
        public override void StartedNewGame()
        {
            Map pocketMap = MapGen.CreatePocketMap();
            MapGen.GenColonistsOnMap(pocketMap);
            CleanUp.Clean();
        }   
    }
}
