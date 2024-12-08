using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

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
    public class SocketComp : MapComponent
    {
        private System.Net.Sockets.TcpClient client;
        private System.IO.StreamWriter writer;
        private System.IO.StreamReader reader;

        [Serializable]
        public class GameState
        {
            public int Pawns;
            public int Timestamp;
        }


        public SocketComp(Map map) : base(map)
        {
            try
            {
                client = new System.Net.Sockets.TcpClient("localhost", 5000);
                var stream = client.GetStream();
                writer = new System.IO.StreamWriter(stream);
                reader = new System.IO.StreamReader(stream);
            }
            catch (Exception e)
            {
                Log.Error($"Failed to initialize socket connection: {e.Message}");
            }
        }

        public override void FinalizeInit()
        {
            SendGameState();
        }

        public void SendGameState()
        {
            try
            {
                // Collect game state

                var gameState = new GameState();
                gameState.Pawns = map.mapPawns.AllPawns.Count();
                gameState.Timestamp = Find.TickManager.TicksGame;

                // Convert to JSON and send
                string jsonState = JsonUtility.ToJson(gameState);
                Log.Message($"Sending game state: {jsonState}");
                writer.WriteLine(jsonState);
                writer.Flush();
            }
            catch (Exception e)
            {
                Log.Error($"Failed to send game state: {e.Message}");
            }
        }

        public override void MapComponentTick()
        {
            if (Find.TickManager.TicksGame % 60 == 0) // Update every second
            {
                SendGameState();
            }
        }

        public override void MapComponentOnGUI()
        {
            // Check for incoming messages from Python
            if (client.Available > 0)
            {
                try
                {
                    string message = reader.ReadLine();
                    // Handle incoming message here
                    Messages.Message("Received message from agent", null, MessageTypeDefOf.PositiveEvent);
                }
                catch (Exception e)
                {
                    Log.Error($"Failed to read message: {e.Message}");
                }
            }
        }
    }
}
