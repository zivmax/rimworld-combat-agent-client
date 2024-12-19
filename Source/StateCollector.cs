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

        public static readonly MapState mapStateCache = new MapState(0, 0);


        public static void Reset()
        {
            pawnStatesCache.Clear();
            mapStateCache.Clear();
        }

        // Pawn-related methods
        public static PawnStates CollectPawnStates()
        {
            pawnStatesCache.Clear();

            ProcessLivingPawns();
            ProcessCorpses();

            return pawnStatesCache;
        }

        private static void ProcessLivingPawns()
        {
            foreach (Pawn pawn in Map.mapPawns.AllHumanlike)
            {
                bool isIncapable = pawn.DeadOrDowned || pawn.InMentalState || pawn.InAggroMentalState;
                // For each ally, we also check if he's been drafted
                if (pawn.Faction == Faction.OfPlayer)
                {
                    isIncapable = isIncapable || !pawn.Drafted;
                }

                pawnStatesCache[pawn.LabelShort] = CreatePawnState(
                    pawn.LabelShort,
                    pawn.Faction == Faction.OfPlayer,
                    pawn.Position,
                    isIncapable,
                    PawnUtility.IsFighting(pawn),
                    isIncapable ? "" : pawn.equipment?.Primary?.LabelShort ?? "",
                    isIncapable ? null : GetCombatStats(pawn),
                    isIncapable ? null : GetHealthStats(pawn)
                );


            }
        }

        private static void ProcessCorpses()
        {
            foreach (Thing thing in Map.listerThings.ThingsInGroup(ThingRequestGroup.Corpse))
            {
                if (thing is Corpse corpse && corpse.InnerPawn.RaceProps.Humanlike)
                {
                    pawnStatesCache[corpse.InnerPawn.LabelShort] = CreatePawnState(
                        corpse.InnerPawn.LabelShort,
                        corpse.InnerPawn.Faction == Faction.OfPlayer,
                        corpse.Position,
                        isIncapable: true,
                        isAiming: false,
                        equipment: "",
                        combatStats: null,
                        healthStats: null
                    );
                }
            }
        }

        private static PawnState CreatePawnState(
            string label,
            bool isAlly,
            IntVec3 position,
            bool isIncapable,
            bool isAiming,
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
                IsIncapable = isIncapable,
                IsAiming = isAiming,
                Equipment = equipment,
                CombatStats = combatStats ?? DefaultCombatStats(),
                HealthStats = healthStats ?? DefaultHealthStats(),
            };
        }

        private static Dictionary<string, float> GetCombatStats(Pawn pawn)
        {
            return new Dictionary<string, float>
            {
                { "MeleeDPS", pawn.GetStatValue(StatDefOf.MeleeDPS) },
                { "ShootingACC", pawn.GetStatValue(StatDefOf.ShootingAccuracyPawn) },
                { "MoveSpeed", pawn.GetStatValue(StatDefOf.MoveSpeed) },
            };
        }

        private static Dictionary<string, float> DefaultCombatStats()
        {
            return new Dictionary<string, float>
            {
                { "MeleeDPS", 0 },
                { "ShootingACC", 0 },
                { "MoveSpeed", 0 },
            };
        }

        private static Dictionary<string, float> GetHealthStats(Pawn pawn)
        {
            return new Dictionary<string, float>
            {
                { "PainTotal", pawn.health.hediffSet.PainTotal },
                { "BloodLoss", pawn.health.hediffSet.BleedRateTotal },
                { "Consciousness", pawn.health.capacities.GetLevel(PawnCapacityDefOf.Consciousness) },
                { "MoveAbility", pawn.health.capacities.GetLevel(PawnCapacityDefOf.Moving) }
            };
        }

        private static Dictionary<string, float> DefaultHealthStats()
        {
            return new Dictionary<string, float>
            {
                { "PainTotal", 0 },
                { "BloodLoss", 0 },
                { "Consciousness", 0 },
                { "MoveAbility", 0 }
            };
        }



        // Map-related methods
        public static MapState CollectMapState()
        {
            mapStateCache.Clear();
            mapStateCache.Width = Map.Size.x;
            mapStateCache.Height = Map.Size.z;

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
        public static GameStatus CheckGameStatus(bool countBlackMan = false)
        {
            var capableAllies = pawnStatesCache.Values.Count(pawn => pawn.IsAlly && !pawn.IsIncapable);
            var capableEnemies = pawnStatesCache.Values.Count(pawn => !pawn.IsAlly && !pawn.IsIncapable);

            int expectedPawnCount = countBlackMan ? Config.TeamSize * 2 + 1 : Config.TeamSize * 2;

            if (capableEnemies == 0)
            {
                return GameStatus.WIN;
            }

            if (capableAllies == 0 || pawnStatesCache.Count > expectedPawnCount)
            {
                return GameStatus.LOSE;
            }

            return GameStatus.RUNNING;
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
