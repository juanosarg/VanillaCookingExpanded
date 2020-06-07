using System;
using Verse;
using Verse.AI;
using RimWorld;

namespace VanillaCookingExpanded
{
    public class WorkGiver_InsertSoup : WorkGiver_Scanner
    {
        private static string NoSoupFound;

        public override ThingRequest PotentialWorkThingRequest
        {
            get
            {
                return ThingRequest.ForDef(ThingDef.Named("VCE_ElectricPot"));
            }
        }

        public override PathEndMode PathEndMode
        {
            get
            {
                return PathEndMode.Touch;
            }
        }

        public static void ResetStaticData()
        {

            WorkGiver_InsertSoup.NoSoupFound = "VCE_NoSoupsFound".Translate();
        }


        public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            Building_ElectricPot building_pot = t as Building_ElectricPot;
            if (building_pot == null || !building_pot.ExpectingSoup || !building_pot.StartInsertionJobs)
            {
                return false;
            }

            if (!t.IsForbidden(pawn))
            {
                LocalTargetInfo target = t;
                if (pawn.CanReserve(target, 1, 1, null, forced))
                {
                    if (pawn.Map.designationManager.DesignationOn(t, DesignationDefOf.Deconstruct) != null)
                    {
                        return false;
                    }
                    if (this.FindSoup(pawn, building_pot.theSoupIAmGoingToInsert, building_pot) == null)
                    {
                        JobFailReason.Is(WorkGiver_InsertSoup.NoSoupFound, null);
                        return false;
                    }
                    return !t.IsBurning();
                }
            }
            return false;
        }

        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            Building_ElectricPot building_pot = (Building_ElectricPot)t;
            Thing t2 = this.FindSoup(pawn, building_pot.theSoupIAmGoingToInsert, building_pot);
            return new Job(DefDatabase<JobDef>.GetNamed("VCE_InsertSoup", true), t, t2);
        }

        private Thing FindSoup(Pawn pawn, string theSoupIAmGoingToInsert, Building_ElectricPot building_pot)
        {
            Predicate<Thing> predicate = (Thing x) => !x.IsForbidden(pawn) && pawn.CanReserve(x, 1, 1, null, false);
            IntVec3 position = pawn.Position;
            Map map = pawn.Map;
            ThingRequest thingReq = ThingRequest.ForDef(ThingDef.Named(theSoupIAmGoingToInsert));
            PathEndMode peMode = PathEndMode.ClosestTouch;
            TraverseParms traverseParams = TraverseParms.For(pawn, Danger.Deadly, TraverseMode.ByPawn, false);
            Predicate<Thing> validator = predicate;
            return GenClosest.ClosestThingReachable(position, map, thingReq, peMode, traverseParams, 9999f, validator, null, 0, -1, false, RegionType.Set_Passable, false);
        }
    }
}


