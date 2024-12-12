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
        public static readonly PawnStates pawnStatesCache = new PawnStates();

        public static MapState mapStateCache = new MapState(0, 0);

        // Pawn-related methods
        public static PawnStates CollectPawnStates()
        {
            foreach (Pawn pawn in Map.mapPawns.AllHumanlike)
            {
                PawnState state;
                if (pawn.DeadOrDowned)
                {
                    state = new PawnState
                    {
                        Label = pawn.LabelShort,
                        IsAlly = pawn.Faction == Faction.OfPlayer,
                        Loc = new Dictionary<string, int>
                        {
                            { "X", pawn.Position.x },
                            { "Y", pawn.Position.z }
                        },
                        Equipment = "",
                        CombatStats = new Dictionary<string, float>
                        {
                            { "MeleeDPS", 0 },
                            { "ShootingACC", 0 },
                            { "MoveSpeed", 0 }
                        },
                        HealthStats = new Dictionary<string, float>
                        {
                            { "PainShock", 0 },
                            { "BloodLoss", 0 },
                        },
                        IsIncapable = true
                    };
                }
                else
                {
                    state = new PawnState
                    {
                        Label = pawn.LabelShort,
                        IsAlly = pawn.Faction == Faction.OfPlayer,
                        Loc = new Dictionary<string, int>
                    {
                        { "X", pawn.Position.x },
                        { "Y", pawn.Position.z }
                    },
                        Equipment = pawn.equipment?.Primary?.LabelShort ?? "",
                        CombatStats = new Dictionary<string, float>
                    {
                        { "MeleeDPS", pawn.GetStatValue(StatDefOf.MeleeDPS) },
                        { "ShootingACC", pawn.GetStatValue(StatDefOf.ShootingAccuracyPawn) },
                        { "MoveSpeed", pawn.GetStatValue(StatDefOf.MoveSpeed) }
                    },
                        HealthStats = new Dictionary<string, float>
                    {
                        { "PainShock", pawn.health.hediffSet.PainTotal },
                        { "BloodLoss", pawn.health.hediffSet.BleedRateTotal },
                    },
                        IsIncapable = false
                    };
                }
                pawnStatesCache[pawn.LabelShort] = state;
            }

            foreach (Thing thing in Map.listerThings.ThingsInGroup(ThingRequestGroup.Corpse))
            {
                if (thing is Corpse corpse && corpse.InnerPawn.RaceProps.Humanlike)
                {
                    var state = new PawnState
                    {
                        Label = corpse.InnerPawn.LabelShort,
                        IsAlly = corpse.InnerPawn.Faction == Faction.OfPlayer,
                        Loc = new Dictionary<string, int>
                        {
                            { "X", corpse.Position.x },
                            { "Y", corpse.Position.z }
                        },
                        Equipment = "",
                        CombatStats = new Dictionary<string, float>
                        {
                            { "MeleeDPS", 0 },
                            { "ShootingACC", 0 },
                            { "MoveSpeed", 0 }
                        },
                        HealthStats = new Dictionary<string, float>
                        {
                            { "PainShock", 0 },
                            { "BloodLoss", 0 },
                        },
                        IsIncapable = true
                    };
                    pawnStatesCache[corpse.InnerPawn.LabelShort] = state;
                }
            }

            return pawnStatesCache;
        }



        // Map-related methods
        public static MapState CollectMapState()
        {
            if (mapStateCache == null || mapStateCache.Width != Map.Size.x || mapStateCache.Height != Map.Size.z)
            {
                mapStateCache = new MapState(Map.Size.x, Map.Size.z);
            }

            foreach (IntVec3 cell in Map.AllCells)
            {
                mapStateCache.Cells[$"({cell.x},{cell.z})"] = new CellState
                {
                    {"IsWall", cell.GetEdifice(Map)?.def.fillPercent >= 1f},
                    {"IsTree", cell.GetPlant(Map)?.def.plant?.IsTree ?? false},
                    {"IsPawn", cell.GetFirstPawn(Map) != null}
                };
            }

            return mapStateCache;
        }

        public static bool IsGameEnding(bool countBlackMan = false)
        {
            if (!countBlackMan)
            {
                // Wait till the man in black arrives
                return pawnStatesCache.Count == 7 && pawnStatesCache.Values.All(pawn => pawn.IsIncapable && pawn.IsAlly);
            }
            else
            {
                // Check if every ally is incapable
                return pawnStatesCache.Values.All(pawn => pawn.IsIncapable && pawn.IsAlly);
            }
        }

        public static void LogPawnState()
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(pawnStatesCache, options);
            Log.Message($"Pawn Data JSON:\n{jsonString}");
        }

        public static void LogMapState()
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(mapStateCache, options);
            Log.Message($"Map Data JSON:\n{jsonString}");
        }

        public static void LogCellState(IntVec3 position)
        {
            var cellState = GetCellState(position);
            if (cellState == null)
            {
                Log.Error($"Cell at position {position} is out of bounds");
                return;
            }

            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(cellState, options);
            Log.Message($"Cell Data JSON:\n{jsonString}");
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

            return mapStateCache.Cells[$"({position.x},{position.z})"];
        }

        public static MapState GetMapState()
        {
            return mapStateCache;
        }
    }
}
