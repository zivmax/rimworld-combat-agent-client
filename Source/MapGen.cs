using System.Collections.Generic;
using System.Linq;
using Verse;
using RimWorld;
using RimWorld.Planet;

namespace CombatAgent
{
    public static class MapGen
    {
        public static void CreatePocketMap()
        {
            IntVec3 mapSize = new IntVec3(30, 1, 30);
            MapParent mapParent = (MapParent)WorldObjectMaker.MakeWorldObject(WorldObjectDefOf.Settlement);
            mapParent.Tile = TileFinder.RandomSettlementTileFor(Faction.OfPlayer);
            mapParent.SetFaction(Faction.OfPlayer);
            Find.WorldObjects.Add(mapParent);
            Current.Game.Scenario = Current.Game.Scenario ?? ScenarioDefOf.Tutorial.scenario;
            Map map = MapGenerator.GenerateMap(mapSize, mapParent, MapGeneratorDefOf.Encounter);

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

            Current.Game.CurrentMap = map;
        }
    }
}