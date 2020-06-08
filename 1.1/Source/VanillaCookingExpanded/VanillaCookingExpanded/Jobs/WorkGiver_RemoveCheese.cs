using System;
using Verse;
using Verse.AI;
using RimWorld;

namespace VanillaCookingExpanded
{
    public class WorkGiver_RemoveCheese : WorkGiver_Scanner
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

            WorkGiver_RemoveCheese.NoMilkFound = "VCE_NoMilksFound".Translate();
        }


        public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            Building_CheesePress building_press = t as Building_CheesePress;
            bool result;
            if (building_press == null || !building_press.CheeseReadyAndWaitingForPickup)
            {
                return false;
            }

            else
            {
                if (!t.IsForbidden(pawn))
                {
                    LocalTargetInfo target = t;
                    if (pawn.CanReserve(target, 1, -1, null, forced))
                    {
                        result = true;
                        return result;
                    }
                }
                result = false;
            }
            return result;
        }

        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            Building_CheesePress building_press = (Building_CheesePress)t;
           
            return new Job(DefDatabase<JobDef>.GetNamed("VCE_RemoveCheese", true), t);
        }

      
    }
}


