namespace CombatAgent
{
    public static class Config
    {
        public static bool AgentControlEnabled { get; set; } = false;
        public static int TeamSize { get; set; } = 1;
        public static int MapSize { get; set; } = 15;
        public static int RestartInterval { get; set; } = 300;
        public static bool MapGenTrees { get; set; } = true;
        public static bool MapGenRuin { get; set; } = true;
        public static int? RandomSeed { get; set; } = 4048;
        public static bool CanFlee { get; set; } = false;
        public static bool ActivelyAttack { get; set; } = true;
        public static float Interval { get; set; } = 1.0f;
        public static int Speed { get; set; } = 1;
    }
}
