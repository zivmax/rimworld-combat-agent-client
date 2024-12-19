namespace CombatAgent
{
    public static class Config
    {
        public static bool AgentControlEnabled { get; set; } = true;
        public static float Interval { get; set; } = 1.0f;
        public static int Speed { get; set; } = 1;
        public static int TeamSize { get; set; } = 3;
    }
}
