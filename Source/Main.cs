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
using RimWorld.SketchGen;

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
            Map pocketMap = CreatePocketMap();
            TeleportColonistsToMap(pocketMap);
        }
        private Map CreatePocketMap()
        {
            IntVec3 mapSize = new IntVec3(30, 1, 30);
            MapParent mapParent = (MapParent)WorldObjectMaker.MakeWorldObject(WorldObjectDefOf.Settlement);
            mapParent.Tile = TileFinder.RandomSettlementTileFor(Faction.OfPlayer);
            mapParent.SetFaction(Faction.OfPlayer);
            Find.WorldObjects.Add(mapParent);
            Current.Game.Scenario = Current.Game.Scenario ?? ScenarioDefOf.Crashlanded.scenario;
            Map map = MapGenerator.GenerateMap(mapSize, mapParent, MapGeneratorDefOf.Base_Player);

            // Set all terrain to soil and clear everything
            foreach (IntVec3 cell in map.AllCells)
            {
                // Remove any mountain/rock roofs
                map.roofGrid.SetRoof(cell, null);

                map.terrainGrid.SetTerrain(cell, TerrainDefOf.Soil);

                // Remove all things in the cell
                List<Thing> things = cell.GetThingList(map).ToList();
                foreach (Thing thing in things)
                {
                    thing.Destroy();
                }

                // Add poplar trees (10% chance per cell)
                if (Rand.Value < 0.075f && cell.Walkable(map))
                {
                    ThingDef treeDef = ThingDefOf.Plant_TreePoplar;
                    Plant tree = (Plant)GenSpawn.Spawn(treeDef, cell, map);
                    tree.Growth = 1f; // Make the tree fully grown
                }
            }

            // Add a more complex L-shaped ruin building
            IntVec3 center = map.Center;
            // Define the L-shape dimensions
            int mainLength = 7;
            int mainWidth = 5;
            int wingLength = 5;
            int wingWidth = 4;

            // Build the main rectangle
            for (int x = -2; x < mainLength - 2; x++)
            {
                for (int z = -2; z < mainWidth - 2; z++)
                {
                    IntVec3 pos = center + new IntVec3(x, 0, z);
                    if (x == -2 || x == mainLength - 3 || z == -2 || z == mainWidth - 3)
                    {
                        if (Rand.Value < 0.7f) // Ruined walls
                        {
                            Thing wall = ThingMaker.MakeThing(ThingDefOf.Wall, ThingDefOf.BlocksGranite);
                            GenSpawn.Spawn(wall, pos, map);
                        }
                    }
                }
            }

            // Build the wing
            for (int x = -2; x < wingWidth - 2; x++)
            {
                for (int z = mainWidth - 2; z < mainWidth + wingLength - 2; z++)
                {
                    IntVec3 pos = center + new IntVec3(x, 0, z);
                    if (x == -2 || x == wingWidth - 3 || z == mainWidth + wingLength - 3)
                    {
                        if (Rand.Value < 0.7f) // Ruined walls
                        {
                            Thing wall = ThingMaker.MakeThing(ThingDefOf.Wall, ThingDefOf.BlocksGranite);
                            GenSpawn.Spawn(wall, pos, map);
                        }
                    }
                }
            }

            // Add some internal walls for rooms
            for (int x = 1; x < 3; x++)
            {
                IntVec3 pos = center + new IntVec3(x, 0, 0);
                if (Rand.Value < 0.8f)
                {
                    Thing wall = ThingMaker.MakeThing(ThingDefOf.Wall, ThingDefOf.BlocksGranite);
                    GenSpawn.Spawn(wall, pos, map);
                }
            }

            return map;
        }

        private void TeleportColonistsToMap(Map targetMap)
        {
            // Delete pet animals
            var pets = PawnsFinder.AllMapsCaravansAndTravelingTransportPods_Alive.Where(p => p.RaceProps.Animal && p.Faction == Faction.OfPlayer).ToList();
            foreach (Pawn pet in pets)
            {
                if (pet.Spawned)
                {
                    pet.Destroy();
                }
            }

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
