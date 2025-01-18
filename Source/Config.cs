using Verse;
using System;

namespace CombatAgent
{
    [Serializable]
    public static class Config
    {
        public static bool AgentControlEnabled { get; set; } = true;
        public static int TeamSize { get; set; } = 1;
        public static int MapSize { get; set; } = 15;
        public static bool MapGenTrees { get; set; } = true;
        public static bool MapGenRuin { get; set; } = true;
        public static int? RandomSeed { get; set; } = 4048;
        public static bool CanFlee { get; set; } = false;
        public static bool ActivelyAttack { get; set; } = false;
        public static bool Headless { get; set; } = false;
        public static float Interval { get; set; } = 1.0f;
        public static int Speed { get; set; } = 1;
        public static string ServerAddress { get; set; } = "localhost";
        public static int ServerPort { get; set; } = 10086;
        public static int fResetInterval = 30;

        public static void GetConfigFromCLIArgs()
        {
            if (GenCommandLine.TryGetCommandLineArg("agent-control", out string agentControl))
            {
                if (bool.TryParse(agentControl, out bool parsedAgentControl))
                    AgentControlEnabled = parsedAgentControl;
                else
                    Log.Error($"Invalid agent-control value: {agentControl}");
            }

            if (GenCommandLine.TryGetCommandLineArg("team-size", out string teamSize))
            {
                if (int.TryParse(teamSize, out int parsedTeamSize))
                    TeamSize = parsedTeamSize;
                else
                    Log.Error($"Invalid team-size value: {teamSize}");
            }

            if (GenCommandLine.TryGetCommandLineArg("map-size", out string mapSize))
            {
                if (int.TryParse(mapSize, out int parsedMapSize))
                    MapSize = parsedMapSize;
                else
                    Log.Error($"Invalid map-size value: {mapSize}");
            }

            if (GenCommandLine.TryGetCommandLineArg("gen-trees", out string genTrees))
            {
                if (bool.TryParse(genTrees, out bool parsedGenTrees))
                    MapGenTrees = parsedGenTrees;
                else
                    Log.Error($"Invalid gen-trees value: {genTrees}");
            }

            if (GenCommandLine.TryGetCommandLineArg("gen-ruins", out string genRuins))
            {
                if (bool.TryParse(genRuins, out bool parsedGenRuins))
                    MapGenRuin = parsedGenRuins;
                else
                    Log.Error($"Invalid gen-ruins value: {genRuins}");
            }

            if (GenCommandLine.TryGetCommandLineArg("seed", out string seed))
            {
                if (int.TryParse(seed, out int parsedSeed))
                    RandomSeed = parsedSeed;
                else
                    Log.Error($"Invalid seed value: {seed}");
            }

            if (GenCommandLine.TryGetCommandLineArg("can-flee", out string canFlee))
            {
                if (bool.TryParse(canFlee, out bool parsedCanFlee))
                    CanFlee = parsedCanFlee;
                else
                    Log.Error($"Invalid can-flee value: {canFlee}");
            }

            if (GenCommandLine.TryGetCommandLineArg("actively-attack", out string attack))
            {
                if (bool.TryParse(attack, out bool parsedAttack))
                    ActivelyAttack = parsedAttack;
                else
                    Log.Error($"Invalid actively-attack value: {attack}");
            }

            if (GenCommandLine.TryGetCommandLineArg("headless", out string headless))
            {
                if (bool.TryParse(headless, out bool parsedHeadless))
                    Headless = parsedHeadless;
                else
                    Log.Error($"Invalid headless value: {headless}");
            }

            if (GenCommandLine.TryGetCommandLineArg("interval", out string interval))
            {
                if (float.TryParse(interval, out float parsedInterval))
                    Interval = parsedInterval;
                else
                    Log.Error($"Invalid interval value: {interval}");
            }

            if (GenCommandLine.TryGetCommandLineArg("speed", out string speed))
            {
                if (int.TryParse(speed, out int parsedSpeed))
                    Speed = parsedSpeed;
                else
                    Log.Error($"Invalid speed value: {speed}");
            }

            if (GenCommandLine.TryGetCommandLineArg("server-addr", out string address))
            {
                ServerAddress = address;
            }

            if (GenCommandLine.TryGetCommandLineArg("server-port", out string portStr))
            {
                if (int.TryParse(portStr, out int port))
                    ServerPort = port;
                else
                    Log.Error($"Invalid port number: {portStr}");
            }

            if (GenCommandLine.TryGetCommandLineArg("f-reset-interval", out string fResetIntervalStr))
            {
                if (int.TryParse(fResetIntervalStr, out int parsedFRInterval))
                    fResetInterval = parsedFRInterval;
                else
                    Log.Error($"Invalid f-reset-interval: {fResetIntervalStr}");
            }
        }

        public static void LogConfig()
        {
            Log.Message($"AgentControlEnabled: {AgentControlEnabled}");
            Log.Message($"TeamSize: {TeamSize}");
            Log.Message($"MapSize: {MapSize}");
            Log.Message($"MapGenTrees: {MapGenTrees}");
            Log.Message($"MapGenRuin: {MapGenRuin}");
            Log.Message($"RandomSeed: {RandomSeed}");
            Log.Message($"CanFlee: {CanFlee}");
            Log.Message($"ActivelyAttack: {ActivelyAttack}");
            Log.Message($"Headless: {Headless}");
            Log.Message($"Interval: {Interval}");
            Log.Message($"Speed: {Speed}");
            Log.Message($"ServerAddress: {ServerAddress}");
            Log.Message($"ServerPort: {ServerPort}");
            Log.Message($"fResetInterval: {fResetInterval}");
        }
    }
}
