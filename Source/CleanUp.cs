using System.Collections.Generic;
using System.Linq;


using Verse;
using RimWorld;
using RimWorld.Planet;

namespace CombatAgent
{
    public static class CleanUp
    {
        public static void CleanWorld()
        {
            // Delete original pawns
            var pawns = Find.WorldPawns.AllPawnsAliveOrDead.ToList();
            foreach (Pawn pawn in pawns)
            {
                pawn.Destroy();
            }

            Current.Game.CurrentMap.Parent.Destroy();

            List<Settlement> settlements = Find.WorldObjects.Settlements.ToList();
            foreach (Settlement settlement in settlements)
            {
                if (!settlement.Faction.IsPlayer)
                {
                    Find.WorldObjects.Remove(settlement);
                }
            }

            Find.Storyteller.incidentQueue.Clear();
            Find.Storyteller.storytellerComps.Clear();
            DefDatabase<IncidentDef>.AllDefs.ToList().ForEach(def =>
            {
                def.baseChance = 0f;
                def.minThreatPoints = 999f;
            });

            DefDatabase<TraderKindDef>.AllDefs.ToList().ForEach(def =>
            {
                def.commonality = 0f;
                def.orbital = false;
                def.requestable = false;
            });

            DefDatabase<PawnKindDef>.AllDefs
                .Where(def => def.RaceProps.Animal)
                .ToList()
                .ForEach(def =>
                {
                    def.canArriveManhunter = false;
                    def.wildGroupSize = IntRange.zero;
                });
        }

        public static void CleanCurrentMap()
        {
            // Clean up all things on the map
            List<Thing> things = Current.Game.CurrentMap.listerThings.AllThings.ToList();
            foreach (Thing thing in things)
            {
                thing.Destroy();
            }


            // Clean up all pawns on the map
            List<Pawn> mapPawns = Current.Game.CurrentMap.mapPawns.AllPawns.ToList();
            foreach (Pawn pawn in mapPawns)
            {
                pawn.Destroy();
            }
        }
    }
}
