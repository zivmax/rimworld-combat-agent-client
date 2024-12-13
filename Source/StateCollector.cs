using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

using Verse;
using RimWorld;

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
            // Process living pawns
            foreach (Pawn pawn in Map.mapPawns.AllHumanlike)
            {
                pawnStatesCache[pawn.LabelShort] = CreatePawnState(
                    pawn.LabelShort,
                    pawn.Faction == Faction.OfPlayer,
                    pawn.Position,
                    isIncapable: pawn.DeadOrDowned,
                    equipment: pawn.DeadOrDowned ? "" : pawn.equipment?.Primary?.LabelShort ?? "",
                    combatStats: pawn.DeadOrDowned ? null : new Dictionary<string, float>
                    {
                        { "MeleeDPS", pawn.GetStatValue(StatDefOf.MeleeDPS) },
                        { "ShootingACC", pawn.GetStatValue(StatDefOf.ShootingAccuracyPawn) },
                        { "MoveSpeed", pawn.GetStatValue(StatDefOf.MoveSpeed) }
                    },
                    healthStats: pawn.DeadOrDowned ? null : new Dictionary<string, float>
                    {
                        { "PainTotal", pawn.health.hediffSet.PainTotal },
                        { "BloodLoss", pawn.health.hediffSet.BleedRateTotal },
                    }
                );
            }

            // Process corpses
            foreach (Thing thing in Map.listerThings.ThingsInGroup(ThingRequestGroup.Corpse))
            {
                if (thing is Corpse corpse && corpse.InnerPawn.RaceProps.Humanlike)
                {
                    pawnStatesCache[corpse.InnerPawn.LabelShort] = CreatePawnState(
                        corpse.InnerPawn.LabelShort,
                        corpse.InnerPawn.Faction == Faction.OfPlayer,
                        corpse.Position,
                        isIncapable: true,
                        equipment: "",
                        combatStats: null,
                        healthStats: null
                    );
                }
            }

            return pawnStatesCache;
        }

        private static PawnState CreatePawnState(
            string label,
            bool isAlly,
            IntVec3 position,
            bool isIncapable,
            string equipment,
            Dictionary<string, float> combatStats = null,
            Dictionary<string, float> healthStats = null)
        {
            return new PawnState
            {
                Label = label,
                IsAlly = isAlly,
                Loc = new Dictionary<string, int>
                {
                    { "X", position.x },
                    { "Y", position.z }
                },
                Equipment = equipment,
                CombatStats = combatStats ?? new Dictionary<string, float>
                {
                    { "MeleeDPS", 0 },
                    { "ShootingACC", 0 },
                    { "MoveSpeed", 0 }
                },
                HealthStats = healthStats ?? new Dictionary<string, float>
                {
                    { "PainTotal", 0 },
                    { "BloodLoss", 0 },
                },
                IsIncapable = isIncapable
            };
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
                    Loc = new Dictionary<string, int>
                    {
                        { "X", cell.x },
                        { "Y", cell.z }
                    },
                    IsWall = cell.GetEdifice(Map)?.def.fillPercent >= 1f,
                    IsTree = cell.GetPlant(Map)?.def.plant?.IsTree ?? false,
                    IsPawn = cell.GetFirstPawn(Map) != null
                };
            }

            return mapStateCache;
        }

        public static bool IsGameEnding(bool countBlackMan = false)
        {
            var capableAllies = pawnStatesCache.Values.Where(pawn => pawn.IsAlly && !pawn.IsIncapable).ToList();

            if (countBlackMan)
            {
                // Wait till the man in black arrives
                return capableAllies.Count == 0 && pawnStatesCache.Count == 7;
            }
            else
            {
                // Check if every ally is incapable
                return capableAllies.Count == 0 && pawnStatesCache.Count == 6;
            }
        }

        public static void LogPawnStates()
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
