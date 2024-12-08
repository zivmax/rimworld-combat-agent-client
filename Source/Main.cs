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
    public class PawnInfoLogger : MapComponent
    {
        public PawnInfoLogger(Map map) : base(map) { }

        public override void MapComponentTick()
        {
            base.MapComponentTick();

            if (Find.TickManager.TicksGame % 1000 == 0) // Log every 1000 ticks
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Pawn positions:");

                foreach (Pawn pawn in map.mapPawns.AllPawns)
                {
                    if (pawn.RaceProps.Humanlike)
                    {
                        sb.AppendLine($"{pawn.LabelShort}: {pawn.Position}");
                        if (pawn.apparel != null && pawn.apparel.WornApparel != null)
                        {
                            sb.AppendLine($"  Wearing: {string.Join(", ", pawn.apparel.WornApparel.Select(a => a.LabelShort))}");
                        }
                        if (pawn.equipment != null && pawn.equipment.Primary != null)
                        {
                            sb.AppendLine($"  Equipment: {pawn.equipment.Primary.LabelShort}");
                        }
                        sb.AppendLine($"  Combat Stats:");
                        sb.AppendLine($"    Melee Hit Chance: {pawn.GetStatValue(StatDefOf.MeleeHitChance):F2}");
                        sb.AppendLine($"    Melee Dodge Chance: {pawn.GetStatValue(StatDefOf.MeleeDodgeChance):F2}");
                        sb.AppendLine($"    Melee DPS: {pawn.GetStatValue(StatDefOf.MeleeDPS):F2}");
                        sb.AppendLine($"    Shooting Accurancy: {pawn.GetStatValue(StatDefOf.ShootingAccuracyPawn):F2}");
                        sb.AppendLine($"    Aiming Time: {pawn.GetStatValue(StatDefOf.AimingDelayFactor):F2}");
                        sb.AppendLine($"    Move Speed: {pawn.GetStatValue(StatDefOf.MoveSpeed):F2}");
                        sb.AppendLine($"  Health Stats:");
                        sb.AppendLine($"    Pain Shock: {pawn.health.hediffSet.PainTotal:F2}");
                        sb.AppendLine($"    Blood Loss: {pawn.health.hediffSet.BleedRateTotal:F2}");
                    }
                }

                Log.Message(sb.ToString());
            }
        }
    }
}
