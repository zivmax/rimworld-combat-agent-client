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

        public static void PerformAction(GameAction action)
        {
            foreach (PawnAction pawnAction in action.PawnActions.Values)
            {
                Pawn pawn = Find.CurrentMap.mapPawns.AllPawnsSpawned.FirstOrDefault(p => p.LabelShort == pawnAction.Lable);
                if (pawn != null)
                {
                    IntVec3 cell = new IntVec3(pawnAction.X, 0, pawnAction.Y);
                    if (pawn.Drafted)
                    {
                        pawn.jobs.StartJob(new Job(JobDefOf.Goto, cell), JobCondition.InterruptForced);
                    }
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
