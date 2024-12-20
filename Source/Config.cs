namespace CombatAgent
{
    public static class Config
    {
        public static bool AgentControlEnabled { get; set; } = true;
        public static int TeamSize { get; set; } = 1;
        public static int MapSize { get; set; } = 20;
        public static bool MapGenTrees { get; set; } = false;
        public static bool MapGenRuin { get; set; } = false;
        public static float Interval { get; set; } = 1.0f;
        public static int Speed { get; set; } = 1;
    }
}
