using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

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

        private static MapState mapStateCache = new MapState(0, 0);

        // Pawn-related methods
        public static void CollectPawnData()
        {
            pawnStatesCache.Clear();
            foreach (Pawn pawn in Map.mapPawns.AllPawns.Where(p => p.RaceProps.Humanlike))
            {
                var state = new PawnState
                {
                    Label = pawn.LabelShort,
                    Position = new Dictionary<string, int>
                    {
                        { "x", pawn.Position.x },
                        { "y", pawn.Position.z }
                    },
                    Equipment = pawn.equipment?.Primary?.LabelShort ?? "",
                    CombatStats = new Dictionary<string, float>
                    {
                        { "meleeDPS", pawn.GetStatValue(StatDefOf.MeleeDPS) },
                        { "shootingAccuracy", pawn.GetStatValue(StatDefOf.ShootingAccuracyPawn) },
                        { "moveSpeed", pawn.GetStatValue(StatDefOf.MoveSpeed) }
                    },
                    HealthStats = new Dictionary<string, float>
                    {
                        { "painShock", pawn.health.hediffSet.PainTotal },
                        { "bloodLoss", pawn.health.hediffSet.BleedRateTotal },
                        { "isDowned", pawn.Downed ? 1 : 0 },
                        { "isDead", pawn.Dead ? 1 : 0 }
                    }
                };

                pawnStatesCache[pawn.LabelShort] = state;
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
                mapStateCache.Cells[$"[{cell.x},{cell.z}]"] = new CellState
                {
                    {"building",  Map.edificeGrid[cell]?.def.defName ?? ""},
                    {"hasPawn", (Map.thingGrid.ThingAt<Pawn>(cell) != null).ToString()}
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
            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(mapStateCache, options);
            Log.Message($"Map Data JSON:\n{jsonString}");
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

            return mapStateCache.Cells[$"[{position.x},{position.z}]"];
        }

        public static MapState GetMapState()
        {
            return mapStateCache;
        }
    }
}
