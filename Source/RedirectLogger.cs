using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;

using UnityEngine;
using Verse;
using Verse.AI;
using Verse.AI.Group;
using Verse.Sound;
using Verse.Noise;
using Verse.Grammar;
using RimWorld;
using RimWorld.Planet;

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
            SocketClient.SendMessage(JsonSerializer.Serialize(log));
        }


    }


}