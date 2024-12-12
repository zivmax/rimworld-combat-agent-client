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
        private readonly Game game;
        public CombatAgentMain(Game game) { this.game = game; }
        public override void GameComponentTick()
        {
            // Send game state to server every 60 ticks (1 second)
            if (Find.TickManager.TicksGame % 60 == 0)
            {
                var state = new GameState
                {
                    MapState = StateCollector.CollectMapState(),
                    PawnStates = StateCollector.CollectPawnStates(),
                    Tick = Find.TickManager.TicksGame,
                    GameEnding = StateCollector.IsGameEnding()
                };
                
                try
                {
                    SocketClient.SendGameState(state);
                    Log.Message("Sent game state to server");
                }
                catch (Exception ex)
                {
                    Log.Error($"Failed to send game state to server: {ex.Message}");
                }
            }
        }
        public override void StartedNewGame()
        {
            DebugViewSettings.neverForceNormalSpeed = true;
            CleanUp.Clean();
            MapGen.CreatePocketMap();
            PawnsGen.GenPawns();
            CameraJumper.TryJump(new GlobalTargetInfo(Find.CurrentMap.Center, Find.CurrentMap));
        }
    }
}
