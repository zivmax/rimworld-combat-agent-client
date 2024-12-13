using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;

using Verse;


namespace CombatAgent
{
    public static class SocketClient
    {
        private static System.Net.Sockets.TcpClient client;
        private static System.IO.StreamWriter writer;
        private static System.IO.StreamReader reader;
        private static bool initialized = false;

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

        private static DataPak ReceiveData()
        {
            try
            {
                if (client == null || !client.Connected || reader == null)
                {
                    Reconnect();
                    return null;
                }
                string message = reader.ReadLine();
                if (string.IsNullOrEmpty(message))
                {
                    return null;
                }
                return JsonSerializer.Deserialize<DataPak>(message);
            }
            catch (Exception e)
            {
                Log.Error($"Failed to receive action: {e.Message}");
                Reconnect();
                return null;
            }
        }

        public static void SendGameState(GameState gameState)
        {
            var data = new DataPak
            {
                Type = "GameState",
                Data = gameState
            };
            SendData(data);
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
            DataPak data;
            try
            {
                data = ReceiveData();
                if (data == null || string.IsNullOrEmpty(data.Type))
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                Log.Error($"Failed to receive action: {e.Message}");
                Reconnect();
                return null;
            }

            if (data.Type == "GameAction")
            {
                return JsonSerializer.Deserialize<GameAction>(data.Data.ToString());
            }
            else
            {
                Log.Error($"Received invalid data type: {data.Type}");
                return null;
            }
        }

        public static void ReceiveMessage()
        {
            try
            {
                string message = reader.ReadLine();
            }
            catch (Exception e)
            {
                Log.Error($"Failed to read message: {e.Message}");
                Reconnect();
            }
        }
    }
}
