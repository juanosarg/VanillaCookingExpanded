using System;
using Verse;
using Verse.AI;
using RimWorld;

namespace VanillaCookingExpanded
{
    public class WorkGiver_InsertMilk : WorkGiver_Scanner
    {
        private static string NoMilkFound;

        public override ThingRequest PotentialWorkThingRequest
        {
            get
            {
                return ThingRequest.ForDef(ThingDef.Named("VCE_CheesePress"));
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

            WorkGiver_InsertMilk.NoMilkFound = "VCE_NoMilksFound".Translate();
        }


        public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            Building_CheesePress building_press = t as Building_CheesePress;
            if (building_press == null || !building_press.ExpectingMilk || !building_press.StartInsertionJobs)
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
                    if (this.FindMilk(pawn, building_press.theMilkIAmGoingToInsert, building_press) == null)
                    {
                        JobFailReason.Is(WorkGiver_InsertMilk.NoMilkFound, null);
                        return false;
                    }
                    return !t.IsBurning();
                }
            }
            return false;
        }

        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            Building_CheesePress building_press = (Building_CheesePress)t;
            Thing t2 = this.FindMilk(pawn, building_press.theMilkIAmGoingToInsert, building_press);
            return new Job(DefDatabase<JobDef>.GetNamed("VCE_InsertMilk", true), t, t2);
        }

        private Thing FindMilk(Pawn pawn, string theMilkIAmGoingToInsert, Building_CheesePress building_press)
        {
            Predicate<Thing> predicate = (Thing x) => !x.IsForbidden(pawn) && pawn.CanReserve(x, 1, 1, null, false);
            IntVec3 position = pawn.Position;
            Map map = pawn.Map;
            ThingRequest thingReq = ThingRequest.ForDef(ThingDef.Named(theMilkIAmGoingToInsert));
            PathEndMode peMode = PathEndMode.ClosestTouch;
            TraverseParms traverseParams = TraverseParms.For(pawn, Danger.Deadly, TraverseMode.ByPawn, false);
            Predicate<Thing> validator = predicate;
            return GenClosest.ClosestThingReachable(position, map, thingReq, peMode, traverseParams, 9999f, validator, null, 0, -1, false, RegionType.Set_Passable, false);
        }
    }
}


