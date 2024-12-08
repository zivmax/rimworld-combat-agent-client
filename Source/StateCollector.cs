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
    public class PawnInfoCollector : MapComponent
    {
        public PawnInfoCollector(Map map) : base(map) { }

        private class PawnData
        {
            public string Label;
            public IntVec3 Position;
            public List<string> Apparel;
            public string Equipment;
            public Dictionary<string, float> CombatStats;
            public Dictionary<string, float> HealthStats;
        }

        private Dictionary<string, PawnData> pawnDataCache = new Dictionary<string, PawnData>();

        private void CollectPawnData()
        {
            pawnDataCache.Clear();
            foreach (Pawn pawn in map.mapPawns.AllPawns.Where(p => p.RaceProps.Humanlike))
            {
                var data = new PawnData
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
                pawnDataCache[pawn.LabelShort] = data;
            }
        }


        public override void MapComponentTick()
        {
            if (Find.TickManager.TicksGame % 1000 == 0)
            {
                CollectPawnData();
                LogPawnData();
            }
        }

        private void LogPawnData()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Collected pawn data:");

            foreach (var pawnData in pawnDataCache)
            {
                sb.AppendLine($"{pawnData.Key}:");
                sb.AppendLine($"  Position: {pawnData.Value.Position}");
                sb.AppendLine($"  Wearing: {string.Join(", ", pawnData.Value.Apparel)}");
                sb.AppendLine($"  Equipment: {pawnData.Value.Equipment}");

                sb.AppendLine($"  Combat Stats:");
                foreach (var stat in pawnData.Value.CombatStats)
                {
                    sb.AppendLine($"    {stat.Key}: {stat.Value:F2}");
                }

                sb.AppendLine($"  Health Stats:");
                foreach (var stat in pawnData.Value.HealthStats)
                {
                    sb.AppendLine($"    {stat.Key}: {stat.Value:F2}");
                }
            }
            Log.Message(sb.ToString());
        }
    }

    public class MapInfoCollector : MapComponent
    {
        public MapInfoCollector(Map map) : base(map) { }

        private class CellData
        {
            public string Terrain;
            public string Building;
            public List<string> Items;
            public float PathCost;
        }

        private CellData[,] mapDataCache;

        private void CollectMapData()
        {
            if (mapDataCache == null || mapDataCache.GetLength(0) != map.Size.x || mapDataCache.GetLength(1) != map.Size.z)
            {
                mapDataCache = new CellData[map.Size.x, map.Size.z];
            }

            foreach (IntVec3 cell in map.AllCells)
            {
                mapDataCache[cell.x, cell.z] = new CellData
                {
                    Terrain = map.terrainGrid.TerrainAt(cell).defName,
                    Building = map.edificeGrid[cell]?.def.defName ?? "",
                    Items = map.thingGrid.ThingsListAt(cell)
                        .Where(t => t.def.category == ThingCategory.Item)
                        .Select(t => t.def.defName)
                        .ToList(),
                    PathCost = map.pathing.For(TraverseMode.PassAllDestroyableThings).pathGrid.PerceivedPathCostAt(cell)
                };
            }
        }

        public override void MapComponentTick()
        {
            if (Find.TickManager.TicksGame % 1000 == 0)
            {
                CollectMapData();
                LogMapData();
            }
        }

        private void LogMapData()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Map Data Sample (10x10 area from origin):");

            int sampleSize = 10;
            for (int z = 0; z < sampleSize && z < map.Size.z; z++)
            {
                for (int x = 0; x < sampleSize && x < map.Size.x; x++)
                {
                    var cell = mapDataCache[x, z];
                    sb.AppendLine($"Cell [{x},{z}]: Terrain={cell.Terrain}, Building={cell.Building}, Items={string.Join(",", cell.Items)}, PathCost={cell.PathCost}");
                }
            }
            Log.Message(sb.ToString());
        }
    }
}
