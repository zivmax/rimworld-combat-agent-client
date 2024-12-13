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
        private static Queue<DataPak> dataPakQueue = new Queue<DataPak>();

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


        private static bool IsConnected()
        {
            try
            {
                return client != null && client.Connected &&
                       !(client.Client.Poll(1, System.Net.Sockets.SelectMode.SelectRead) &&
                     client.Client.Available == 0);
            }
            catch
            {
                return false;
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
                while (!IsConnected())
                {
                    Log.Warning("Socket not connected, attempting to reconnect...");
                    Reconnect();
                }

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

        private static void ProcessIncomingData()
        {
            try
            {
                while (!IsConnected())
                {
                    Log.Warning("Socket not connected, attempting to reconnect...");
                    Reconnect();
                }

                while (client.GetStream().DataAvailable)
                {
                    string message = reader.ReadLine();
                    if (!string.IsNullOrEmpty(message))
                    {
                        var data = JsonSerializer.Deserialize<DataPak>(message);
                        dataPakQueue.Enqueue(data);
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
            ProcessIncomingData();

            var messages = dataPakQueue.ToList();
            for (int i = 0; i < messages.Count; i++)
            {
                if (messages[i].Type == type)
                {
                    dataPakQueue = new Queue<DataPak>(messages.Take(i).Concat(messages.Skip(i + 1)));
                    return messages[i];
                }
            }
            return null;
        }

        public static void SendGameState(GameState state)
        {
            var data = new DataPak
            {
                Type = "GameState",
                Data = state
            };
            SendData(data);
        }

        public static void SendLog(string log)
        {
            var data = new DataPak
            {
                Type = "Log",
                Data = log
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

        public static bool ReceiveReset()
        {
            var data = GetNextMessageOfType("Reset");
            return data != null;
        }
    }
}
