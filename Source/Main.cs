using Verse;
using RimWorld;

namespace CombatAgent
{
    public class CombatAgentMain : GameComponent
    {
        public CombatAgentMain(Game game) { }

        private Map trainMap;
        private static bool reseting = true;
        private static int qResetTimes = 0;

        private static int Second(float second)
        {
            return (int)(second * 60);
        }

        private static void Reset()
        {
            reseting = true;
            Log.Message("Starting reset process...");
            if (qResetTimes >= Config.fResetInterval && Config.fResetInterval != 0)
            {
                FullReset();
                qResetTimes = 0;
            }
            else
            {
                QuickReset();
            }
            Log.Clear();
            Messages.Clear();
            reseting = false;
        }

        private static void FullReset()
        {
            Log.Message("Full reset: Resetting state collector");
            StateCollector.Reset();
            Current.Game.CurrentMap.Parent.Destroy();
            Root_Play.SetupForQuickTestPlay();
            PageUtility.InitGameStart();
        }

        private static void QuickReset()
        {
            Log.Message("Quick reset: Resetting state collector");
            StateCollector.Reset();
            CleanUp.CleanCurrentMap();
            MapGen.RefreshMap(Current.Game.CurrentMap);
            PawnsGen.GenPawns(Current.Game.CurrentMap);
            PawnController.DraftAllAllies();
            Find.TickManager.DebugSetTicksGame(Second(0));
            Find.TickManager.ResetSettlementTicks();
            qResetTimes++;
            Log.Message("Quick reset completed");
        }

        private static void PACycle()
        {
            Log.Message($"Starting PA Cycle at tick {Find.TickManager.TicksGame}");

            var state = new GameState
            {
                MapState = StateCollector.CollectMapState(),
                PawnStates = StateCollector.CollectPawnStates(),
                Tick = Find.TickManager.TicksGame,
                Status = StateCollector.CheckGameStatus()
            };

            Log.Message($"Waiting for agent response");
            AgentResponse res = SocketClient.SendState(state);

            if (Config.AgentControlEnabled)
            {
                if (res.Reset)
                {
                    Reset();
                    return;
                }
                PawnController.PerformAction(res.Action.PawnActions);
            }
            else if (state.Status != GameStatus.RUNNING)
            {
                Log.Message("Game not running, initiating reset");
                Reset();
            }

            Find.TickManager.CurTimeSpeed = (TimeSpeed)Config.Speed;
        }

        public override void GameComponentTick()
        {
            if (reseting)
                return;

            if (Find.TickManager.TicksGame % Second(Config.Interval) == 0)
            {
                Find.TickManager.Pause();
                PACycle();
                Find.TickManager.CurTimeSpeed = (TimeSpeed)Config.Speed;
            }
        }

        public override void StartedNewGame()
        {
            Log.Message("Starting new game initialization");
            CleanUp.CleanWorld();
            trainMap = MapGen.CreatePocketMap();
            Current.Game.CurrentMap = trainMap;
            PawnsGen.GenPawns(trainMap);
            CameraJumper.TryJump(trainMap.Center, trainMap);
            PawnController.DraftAllAllies();
            reseting = false;
            Log.Message("New game initialization complete");
            PACycle();
        }
    }
}
