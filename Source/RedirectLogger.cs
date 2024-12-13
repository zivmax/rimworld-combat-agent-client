using System.Text.Json;

using UnityEngine;


namespace CombatAgent
{
    public static class RedirectLogger
    {
        private static bool initialized = false;

        public static void Initialize()
        {
            if (!initialized)
            {
                Application.logMessageReceived += RedirectLog;
                initialized = true;
            }
        }

        public class Message
        {
            public string type;
            public string logString;
        }

        private static void RedirectLog(string logString, string stackTrace, LogType type)
        {
            var log = new Message
            {
                type = type.ToString(),
                logString = logString
            };
            SocketClient.SendLog(JsonSerializer.Serialize(log));
        }


    }


}