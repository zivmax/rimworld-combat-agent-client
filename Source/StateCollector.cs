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
    public static class StateCollector
    {
        private static Map Map => Find.CurrentMap;

        // Caches
        private static readonly PawnStates pawnStatesCache = new PawnStates();

        private static MapState mapStateCache;

        // Pawn-related methods
        public static void CollectPawnData()
        {
            pawnStatesCache.Clear();
            foreach (Pawn pawn in Map.mapPawns.AllPawns.Where(p => p.RaceProps.Humanlike))
            {
                var data = new PawnState
                {
                    Label = pawn.LabelShort,
                    Position = pawn.Position,
                    Apparel = pawn.apparel?.WornApparel?.Select(a => a.LabelShort).ToList() ?? new List<string>(),
                    Equipment = pawn.equipment?.Primary?.LabelShort ?? "",
                    CombatStats = new Dictionary<string, float>
                {
                    { "MeleeHitChance", pawn.GetStatValue(StatDefOf.MeleeHitChance) },
                    { "MeleeDodgeChance", pawn.GetStatValue(StatDefOf.MeleeDodgeChance) },
                    { "MeleeDPS", pawn.GetStatValue(StatDefOf.MeleeDPS) },
                    { "ShootingAccuracy", pawn.GetStatValue(StatDefOf.ShootingAccuracyPawn) },
                    { "AimingDelay", pawn.GetStatValue(StatDefOf.AimingDelayFactor) },
                    { "MoveSpeed", pawn.GetStatValue(StatDefOf.MoveSpeed) }
                },
                    HealthStats = new Dictionary<string, float>
                {
                    { "PainShock", pawn.health.hediffSet.PainTotal },
                    { "BloodLoss", pawn.health.hediffSet.BleedRateTotal }
                }
                };

                pawnStatesCache[pawn.LabelShort] = data;
            }
        }



        // Map-related methods
        public static void CollectMapData()
        {
            if (mapStateCache == null || mapStateCache.Width != Map.Size.x || mapStateCache.Height != Map.Size.z)
            {
                mapStateCache = new MapState(Map.Size.x, Map.Size.z);
            }

            foreach (IntVec3 cell in Map.AllCells)
            {
                mapStateCache[cell.x, cell.z] = new CellState
                {
                    Terrain = Map.terrainGrid.TerrainAt(cell).defName,
                    Building = Map.edificeGrid[cell]?.def.defName ?? "",
                    Items = Map.thingGrid.ThingsListAt(cell)
                        .Where(t => t.def.category == ThingCategory.Item)
                        .Select(t => t.def.defName)
                        .ToList(),
                    PathCost = Map.pathing.For(TraverseMode.PassAllDestroyableThings).pathGrid.PerceivedPathCostAt(cell)
                };
            }
        }

        public static void LogPawnData()
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(pawnStatesCache, options);
            Log.Message($"Pawn Data JSON:\n{jsonString}");
        }

        public static void LogMapData()
        {
            var sampleData = new Dictionary<string, CellState>();

            int sampleSize = 10;
            for (int z = 0; z < sampleSize && z < Map.Size.z; z++)
            {
                for (int x = 0; x < sampleSize && x < Map.Size.x; x++)
                {
                    sampleData[$"[{x},{z}]"] = mapStateCache[x, z];
                }
            }

            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(sampleData, options);
            Log.Message($"Map Data Sample (10x10 area from origin):\n{jsonString}");
        }

        public static PawnStates GetPawnStates()
        {
            return pawnStatesCache;
        }

        public static CellState GetCellState(IntVec3 position)
        {
            if (mapStateCache == null ||
                position.x < 0 || position.x >= Map.Size.x ||
                position.z < 0 || position.z >= Map.Size.z)
                return null;

            return mapStateCache[position.x, position.z];
        }

        public static MapState GetMapState()
        {
            return mapStateCache;
        }
    }
}
