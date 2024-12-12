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

        private static void Reconnect()
        {
            try
            {
                client?.Close();
                client = new System.Net.Sockets.TcpClient("localhost", 10086);
                var stream = client.GetStream();
                writer = new System.IO.StreamWriter(stream);
                reader = new System.IO.StreamReader(stream);
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
        public static void SendGameState(GameState gameState)
        {
            var data = new DataPak
            {
                Type = "gameState",
                Data = gameState
            };
            SendData(data);
        }

        public static void SendMessage(string message)
        {
            var data = new DataPak
            {
                Type = "log",
                Data = message
            };
            SendData(data);
        }

        public static void ReceiveMessage()
        {
            try
            {
                string message = reader.ReadLine();
                Log.Message($"Received message: {message}");
            }
            catch (Exception e)
            {
                Log.Error($"Failed to read message: {e.Message}");
                Reconnect();
            }
        }
    }
}
