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

        private static int Second(int second)
        {
            // 60 ticks per second
            return second * 60;
        }

        public override void GameComponentTick()
        {
            if (Find.TickManager.TicksGame % Second(5) == 0)
            {
                // Pause the game
                Find.TickManager.Pause();

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

            if (SocketClient.ReceiveReset())
            {
                Restart();
                return;
            }

            GameAction action = SocketClient.ReceiveAction();
            while (action == null)
            {
                SocketClient.SendGameState(state);
                action = SocketClient.ReceiveAction();
            }

                PawnController.PerformAction(action);

            // Resume the game
            Find.TickManager.CurTimeSpeed = TimeSpeed.Normal;
        }

        public override void GameComponentTick()
        {
            if (Find.TickManager.TicksGame % Second(5) == 0)
            {
                PACycle();
            }
        }

        public override void FinalizeInit()
        {
            PrefInitializer.SetPrefs();
        }

        public override void StartedNewGame()
        {
            PrefInitializer.SetPrefs();
            CleanUp.Clean();
            MapGen.CreatePocketMap();
            PawnsGen.GenPawns();
            CameraJumper.TryJump(new GlobalTargetInfo(Find.CurrentMap.Center, Find.CurrentMap));
            PawnController.DraftAllAllies();
            Find.TickManager.CurTimeSpeed = TimeSpeed.Normal;
        }
    }
}
