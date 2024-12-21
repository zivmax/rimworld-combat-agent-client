namespace CombatAgent
{
    public static class Config
    {
        public static bool AgentControlEnabled { get; set; } = true;
        public static int TeamSize { get; set; } = 3;
        public static int MapSize { get; set; } = 20;
        public static bool MapGenTrees { get; set; } = false;
        public static bool MapGenRuin { get; set; } = true;
        public static int? RandomSeed { get; set; } = null;
        public static float Interval { get; set; } = 1.0f;
        public static int Speed { get; set; } = 1;
    }
}
