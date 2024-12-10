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
    public class CombatAgentMain : GameComponent
    {
        public CombatAgentMain(Game game) { }
        public override void GameComponentTick()
        {
            if (Find.TickManager.TicksGame % 500 == 0)
            {
                StateCollector.CollectPawnState();
                StateCollector.CollectMapState();
                var state = new GameState
                {
                    Map = StateCollector.GetMapState(),
                    Pawns = StateCollector.GetPawnStates(),
                    Tick = Find.TickManager.TicksGame
                };
                StateCollector.LogCellState(IntVec3.Zero);
                SocketClient.SendGameState(state);
            }
        }
        public override void StartedNewGame()
        {
            Map pocketMap = GetOrCreatePocketMap();
            TeleportColonistsToMap(pocketMap);
        }

        private Map GetOrCreatePocketMap()
        {
            IntVec3 mapSize = new IntVec3(20, 1, 20);
            MapParent mapParent = (MapParent)WorldObjectMaker.MakeWorldObject(WorldObjectDefOf.Settlement);
            mapParent.Tile = TileFinder.RandomSettlementTileFor(Faction.OfPlayer);
            mapParent.SetFaction(Faction.OfPlayer);
            Find.WorldObjects.Add(mapParent);
            var genSteps = new List<GenStepWithParams>();
            Current.Game.Scenario = Current.Game.Scenario ?? ScenarioDefOf.Crashlanded.scenario;
            Map map = MapGenerator.GenerateMap(mapSize, mapParent, MapGeneratorDefOf.Base_Player, genSteps, null);

            foreach (IntVec3 cell in map.AllCells)
            {
                map.terrainGrid.SetTerrain(cell, TerrainDefOf.Soil);
            }
            return map;
        }

        private void TeleportColonistsToMap(Map targetMap)
        {
            // Delete original colonists
            var colonists = PawnsFinder.AllMapsCaravansAndTravelingTransportPods_Alive_Colonists.ToList();
            foreach (Pawn colonist in colonists)
            {
                if (colonist.Spawned)
                {
                    colonist.Destroy();
                }
            }

            // Generate 2 new colonists
            for (int i = 0; i < 2; i++)
            {
                Pawn newColonist = PawnGenerator.GeneratePawn(PawnKindDefOf.Colonist, Faction.OfPlayer);
                IntVec3 position = new IntVec3(0, 0, 0);
                GenSpawn.Spawn(newColonist, position, targetMap);
                Log.Message($"Generated new colonist {newColonist.Name} on new map at origin");
            }

            Current.Game.CurrentMap = targetMap;
            Find.CameraDriver.JumpToCurrentMapLoc(targetMap.Center);
        }
    }
}
