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
    public static class CleanUp
    {
        public static void Clean()
        {
            // Delete original pawns
            var pawns = Find.WorldPawns.AllPawnsAliveOrDead.ToList();
            foreach (Pawn pawn in pawns)
            {
                pawn.Destroy();
            }

            var oldMaps = Find.Maps.ToList();
            foreach (var oldMap in oldMaps)
            {
                if (oldMap.Parent is Settlement settlement)
                {
                    settlement.Abandon();
                }
            }

            List<Settlement> settlements = Find.WorldObjects.Settlements.ToList();
            foreach (Settlement settlement in settlements)
            {
                if (!settlement.Faction.IsPlayer)
                {
                    Find.WorldObjects.Remove(settlement);
                }
            }

            Find.Storyteller.incidentQueue.Clear();
            DefDatabase<IncidentDef>.AllDefs.ToList().ForEach(def =>
            {
                def.baseChance = 0f;
                def.minThreatPoints = 999f;
            });
        }
    }
}
