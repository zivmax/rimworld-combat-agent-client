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
using Microsoft.SqlServer.Server;

namespace CombatAgent
{
    public static class SocketClient
    {
        private static int LastCheckTick = -1;

        private static Queue<DataPak> messageQueue = new Queue<DataPak>();

        private static System.Net.Sockets.TcpClient client;
        private static System.IO.StreamWriter writer;
        private static System.IO.StreamReader reader;

        static SocketClient()
        {
            try
            {
                client = new System.Net.Sockets.TcpClient("localhost", 10086);
                var stream = client.GetStream();
                writer = new System.IO.StreamWriter(stream);
                reader = new System.IO.StreamReader(stream);
            }
            catch (Exception e)
            {
                Log.Error($"Failed to initialize socket connection: {e.Message}");
            }
        }

        public static void ResetTick()
        {
            LastCheckTick = -1;
        }

        public static void Initialize()
        {
            if (!initialized)
            {
                initialized = true;
            }
        }

        private static void Reconnect()
        {
            try
            {
                client?.Close();
                client = new System.Net.Sockets.TcpClient("localhost", 10086);
                var stream = client.GetStream();
                writer = new System.IO.StreamWriter(stream);
                reader = new System.IO.StreamReader(stream);
                LastCheckTick = -1;
                Log.Message("Successfully reconnected to socket server");
            }
            catch (Exception e)
            {
                Log.Error($"Failed to reconnect: {e.Message}");
            }
        }

        private class DataPak
        {
            public string Type { get; set; }
            public object Data { get; set; }
        }

        private static void SendData(DataPak data)
        {
            try
            {
                // Convert to JSON and send
                string jsonData = JsonSerializer.Serialize(data);
                writer.WriteLine(jsonData);
                writer.Flush();
            }
            catch (Exception e)
            {
                Log.Error($"Failed to send data: {e.Message}");
                Reconnect();
            }
        }

        private static void ProcessIncomingMessages()
        {
            try
            {
                while (client.GetStream().DataAvailable)
                {
                    string message = reader.ReadLine();
                    if (!string.IsNullOrEmpty(message))
                    {
                        var data = JsonSerializer.Deserialize<DataPak>(message);
                        messageQueue.Enqueue(data);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error($"Failed to process messages: {e.Message}");
            }
        }

        private static DataPak GetNextMessageOfType(string type)
        {
            ProcessIncomingMessages();

            var messages = messageQueue.ToList();
            for (int i = 0; i < messages.Count; i++)
            {
                if (messages[i].Type == type)
                {
                    messageQueue = new Queue<DataPak>(messages.Take(i).Concat(messages.Skip(i + 1)));
                    return messages[i];
                }
            }
            return null;
        }

        public static void SendGameState(GameState gameState)
        {
            if (LastCheckTick >= gameState.Tick)
            {
                Log.Warning("Last check tick is greater than or equal to current tick, skipping...");
                return;
            }

            var data = new DataPak
            {
                Type = "GameState",
                Data = gameState
            };
            SendData(data);
            while (!ReceiveCheck())
            {
                Log.Warning("Failed to receive tick check, resending game state...");
                SendData(data);
            }
        }

        public static void SendMessage(string message)
        {
            var data = new DataPak
            {
                Type = "Log",
                Data = message
            };
            SendData(data);
        }

        public static GameAction ReceiveAction()
        {
            var data = GetNextMessageOfType("GameAction");
            if (data == null)
            {
                return null;
            }
            return JsonSerializer.Deserialize<GameAction>(data.Data.ToString());
        }

        public static bool ReceiveCheck()
        {
            var data = GetNextMessageOfType("TickCheck");
            if (data == null)
            {
                return false;
            }

            try
            {
                LastCheckTick = JsonSerializer.Deserialize<int>(data.Data.ToString());
            }
            catch (Exception e)
            {
                Log.Error($"Failed to parse tick check: {e.Message}");
                return false;
            }

            return true;
        }

        public static bool ReceiveReset()
        {
            var data = GetNextMessageOfType("Reset");
            return data != null;
        }

        public static string ReceiveLog()
        {
            var data = GetNextMessageOfType("Log");
            if (data == null)
            {
                return null;
            }
            return data.Data.ToString();
        }
    }
}
