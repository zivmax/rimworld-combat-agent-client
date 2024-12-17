using System;
using System.Text.Json;

using Verse;


namespace CombatAgent
{
    [Serializable]
    public class AgentResponse
    {
        public GameAction Action { get; set; }
        public bool Reset { get; set; }
        public float Interval { get; set; }
        public int Speed { get; set; }
    }

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



        public static AgentResponse SendState(GameState state)
        {
            var data = new DataPak
            {
                Type = "GameState",
                Data = state
            };
            SendData(data);

            var response = ReceiveData();
            while (response == null)
            {
                SendData(data);
                response = ReceiveData();
            }

            return JsonSerializer.Deserialize<AgentResponse>(JsonSerializer.Serialize(response.Data));
        }

    }
}
