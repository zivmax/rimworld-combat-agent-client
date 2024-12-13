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

        private Map trainMap;
        private static bool restarting = true;

        private static int Second(int second)
        {
            // 60 ticks per second
            return second * 60;
        }

        private static void Restart()
        {
            restarting = true;
            StateCollector.Reset();
            Current.Game.CurrentMap.Parent.Destroy();
            Root_Play.SetupForQuickTestPlay();
            Find.GameInitData.PrepForMapGen();
            Find.Scenario.PreMapGenerate();
            Current.Game.InitNewGame();
        }

        private static void PACycle()
        {
            if (restarting)
            {
                return;
            }

            // Pause the game
            Find.TickManager.Pause();

            var state = new GameState
            {
                MapState = StateCollector.CollectMapState(),
                PawnStates = StateCollector.CollectPawnStates(),
                Tick = Find.TickManager.TicksGame,
                Status = StateCollector.CheckGameStatus()
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
                SocketClient.SendLog("Game is ending, restarting");
                Restart();
                return;
            }

            GameAction action = SocketClient.ReceiveAction();
            while (action == null)
            {
                SocketClient.SendLog("Game is looping...");
                action = SocketClient.ReceiveAction();
            }

            PawnController.PerformAction(action);

            // Resume the game
            Find.TickManager.CurTimeSpeed = TimeSpeed.Ultrafast;
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
            CleanUp.Clean();
            trainMap = MapGen.CreatePocketMap();
            Current.Game.CurrentMap = trainMap;
            PawnsGen.GenPawns(trainMap);
            CameraJumper.TryJump(trainMap.Center, trainMap);
            PawnController.DraftAllAllies();
            restarting = false;
            PACycle();
        }
    }
}
