using Verse;


namespace CombatAgent
{
    public class CombatAgentMain : GameComponent
    {
        public CombatAgentMain(Game game) { }

        private Map trainMap;
        private static bool restarting = true;

        private static int Second(float second)
        {
            // 60 ticks per second
            return (int)(second * 60);
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

            AgentResponse res = SocketClient.SendState(state);

            if (Config.AgentControlEnabled)
            {
                if (res.Reset)
                {
                    Config.Interval = res.Interval;
                    Config.Speed = res.Speed;
                    Restart();
                    return;
                }

                PawnController.PerformAction(res.Action.PawnActions);
            }

            // Resume the game
            Find.TickManager.CurTimeSpeed = (TimeSpeed)Config.Speed;
        }

        public override void GameComponentTick()
        {
            if (Find.TickManager.TicksGame % Second(Config.Interval) == 0)
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
