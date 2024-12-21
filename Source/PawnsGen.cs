using System.Linq;
using Verse;
using RimWorld;
using Verse.AI.Group;

namespace CombatAgent
{
    public static class PawnsGen
    {
        public static void GenPawns(Map map)
        {
            // Generate Ally
            for (int i = 0; i < Config.TeamSize; i++)
            {
                PawnKindDef pawnKind = PawnKindDefOf.Colonist;
                pawnKind.race = ThingDefOf.Human;
                Pawn newAlly = PawnGenerator.GeneratePawn(pawnKind, Faction.OfPlayer);
                newAlly.equipment.DestroyAllEquipment();
                IntVec3 position = new IntVec3(0, 0, 0);
                newAlly.story.Childhood = DefDatabase<BackstoryDef>.AllDefs.FirstOrDefault(b => b.defName == "ColonyChild59");
                newAlly.story.Adulthood = DefDatabase<BackstoryDef>.AllDefs.FirstOrDefault(b => b.defName == "Colonist97");
                newAlly.ageTracker.AgeBiologicalTicks = (30 * 3600000L); // Set age to 30
                for (int num = newAlly.story.traits.allTraits.Count - 1; num >= 0; num--)
                {
                    newAlly.story.traits.RemoveTrait(newAlly.story.traits.allTraits[num]);
                }
                newAlly.health.Reset();
                newAlly.skills.GetSkill(SkillDefOf.Shooting).Level = 5;
                newAlly.skills.GetSkill(SkillDefOf.Melee).Level = 5;
                GenSpawn.Spawn(newAlly, position, map);
                PostPawnSpawn(newAlly);
                Log.Message($"Generated new ally {newAlly.Name} on new map at origin");
            }

            // Generate Enemy
            for (int i = 0; i < Config.TeamSize; i++)
            {
                PawnKindDef pawnKind = PawnKindDefOf.Pirate;
                pawnKind.race = ThingDefOf.Human;
                Faction faction = FactionUtility.DefaultFactionFrom(PawnKindDefOf.Pirate.defaultFactionType);
                Pawn newEnemy = PawnGenerator.GeneratePawn(PawnKindDefOf.Pirate, faction);
                newEnemy.equipment.DestroyAllEquipment();
                IntVec3 position = new IntVec3(map.Size.x - 1, 0, map.Size.z - 1);
                newEnemy.story.Childhood = DefDatabase<BackstoryDef>.AllDefs.FirstOrDefault(b => b.defName == "ColonyChild59");
                newEnemy.story.Adulthood = DefDatabase<BackstoryDef>.AllDefs.FirstOrDefault(b => b.defName == "Colonist97");
                newEnemy.ageTracker.AgeBiologicalTicks = (30 * 3600000L); // Set age to 30
                for (int num = newEnemy.story.traits.allTraits.Count - 1; num >= 0; num--)
                {
                    newEnemy.story.traits.RemoveTrait(newEnemy.story.traits.allTraits[num]);
                }
                newEnemy.health.Reset();
                newEnemy.skills.GetSkill(SkillDefOf.Shooting).Level = 5;
                newEnemy.skills.GetSkill(SkillDefOf.Melee).Level = 5;
                GenSpawn.Spawn(newEnemy, position, map);
                PostPawnSpawn(newEnemy);
                Log.Message($"Generated new enemy {newEnemy.Name} on new map at top right");
            }

            foreach (Pawn pawn in map.mapPawns.AllHumanlike)
            {
                ThingDef smgDef = ThingDef.Named("Gun_HeavySMG");
                Thing smg = ThingMaker.MakeThing(smgDef, null);
                pawn.equipment.AddEquipment((ThingWithComps)smg);

                ThingDef flakVestDef = ThingDef.Named("Apparel_FlakVest");
                ThingDef flakHelmetDef = ThingDef.Named("Apparel_AdvancedHelmet");
                ThingDef flakPantsDef = ThingDef.Named("Apparel_FlakPants");
                ThingDef flakJacketDef = ThingDef.Named("Apparel_FlakJacket");
                Thing flakVest = ThingMaker.MakeThing(flakVestDef, null);
                Thing flakHelmet = ThingMaker.MakeThing(flakHelmetDef, null);
                Thing flakPants = ThingMaker.MakeThing(flakPantsDef, null);
                Thing flakJacket = ThingMaker.MakeThing(flakJacketDef, null);
                pawn.apparel.Wear((Apparel)flakVest, dropReplacedApparel: false);
                pawn.apparel.Wear((Apparel)flakHelmet, dropReplacedApparel: false);
                pawn.apparel.Wear((Apparel)flakPants, dropReplacedApparel: false);
                pawn.apparel.Wear((Apparel)flakJacket, dropReplacedApparel: false);
            }
        }

        private static void PostPawnSpawn(Pawn pawn)
        {
            if (pawn.Spawned && pawn.Faction != null && pawn.Faction != Faction.OfPlayer)
            {
                Lord lord = null;
                if (pawn.Map.mapPawns.SpawnedPawnsInFaction(pawn.Faction).Any((Pawn p) => p != pawn))
                {
                    lord = ((Pawn)GenClosest.ClosestThing_Global(pawn.Position, pawn.Map.mapPawns.SpawnedPawnsInFaction(pawn.Faction), 99999f, (Thing p) => p != pawn && ((Pawn)p).GetLord() != null)).GetLord();
                }
                if (lord == null || !lord.CanAddPawn(pawn))
                {
                    lord = LordMaker.MakeNewLord(pawn.Faction, new LordJob_DefendPoint(pawn.Position), Find.CurrentMap);
                }
                if (lord != null && lord.LordJob.CanAutoAddPawns)
                {
                    lord.AddPawn(pawn);
                }
            }
            pawn.Rotation = Rot4.South;
        }

    }
}