using System.Linq;

using Verse;
using Verse.AI;
using RimWorld;

using System.Collections.Generic;
using Verse.AI.Group;


namespace CombatAgent
{
    public class PawnController
    {
        public static void DraftAllAllies()
        {
            foreach (Pawn pawn in Find.CurrentMap.mapPawns.FreeColonistsSpawned)
            {
                if (pawn.drafter != null)
                {
                    pawn.drafter.Drafted = true;
                    pawn.drafter.FireAtWill = true;
                }
            }
        }

        public static void PerformAction(PawnActions actions)
        {
            if (actions == null)
            {
                return;
            }

            foreach (PawnAction pawnAction in actions.Values)
            {
                Pawn pawn = Find.CurrentMap.mapPawns.AllPawnsSpawned.FirstOrDefault(p => p.LabelShort == pawnAction.Label);
                if (pawn != null)
                {
                    IntVec3 cell = new IntVec3(pawnAction.X, 0, pawnAction.Y);

                    // Check for enemy, tree, or wall at the target position
                    bool invalidPos = Find.CurrentMap.thingGrid.ThingsListAt(cell).Any(thing =>
                        thing is Pawn enemy && enemy.Faction.HostileTo(Faction.OfPlayer) ||
                        thing.def.category == ThingCategory.Plant ||
                        thing.def.category == ThingCategory.Building);

                    // Check if the target position is the same as the pawn's current position
                    invalidPos = invalidPos || (cell.x == pawn.Position.x && cell.z == pawn.Position.z);

                    // Check if the target position is in map bounds
                    invalidPos = invalidPos || cell.x < 0 || cell.x >= Find.CurrentMap.Size.x || cell.z < 0 || cell.z >= Find.CurrentMap.Size.z;

                    if ( !invalidPos && pawn.Drafted)
                    {
                        Job job = JobMaker.MakeJob(JobDefOf.Goto, cell);
                        job.playerForced = true;
                        pawn.jobs.StartJob(job, JobCondition.InterruptForced);
                    }
                }
                else
                {
                    Log.Warning($"Pawn with label {pawnAction.Label} not found");
                }
            }
        }

        // For testing purposes
        public static void PerformRandomAction()
        {
            foreach (Pawn pawn in Find.CurrentMap.mapPawns.FreeColonistsSpawned)
            {
                if (pawn.Drafted)
                {
                    var map = Find.CurrentMap;
                    IntVec3 cell;
                    do
                    {
                        cell = new IntVec3(Rand.Range(0, map.Size.x), 0, Rand.Range(0, map.Size.z));
                    } while (!map.pathFinder.FindPath(pawn.Position, cell, TraverseParms.For(pawn)).Found ||
                            map.pathFinder.FindPath(pawn.Position, cell, TraverseParms.For(pawn)).TotalCost < 40f);
                    pawn.jobs.StartJob(new Job(JobDefOf.Goto, cell), JobCondition.InterruptForced);
                }
            }
        }
    }
}
