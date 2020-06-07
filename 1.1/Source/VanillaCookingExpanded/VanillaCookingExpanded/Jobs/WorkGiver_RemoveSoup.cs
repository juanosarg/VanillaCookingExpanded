using System;
using Verse;
using Verse.AI;
using RimWorld;

namespace VanillaCookingExpanded
{
    public class WorkGiver_RemoveSoup : WorkGiver_Scanner
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

            WorkGiver_RemoveSoup.NoSoupFound = "VCE_NoSoupsFound".Translate();
        }


        public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            Building_ElectricPot building_pot = t as Building_ElectricPot;
            bool result;
            if (building_pot == null || !building_pot.SoupReadyAndWaitingForPickup)
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
            Building_ElectricPot building_pot = (Building_ElectricPot)t;
           
            return new Job(DefDatabase<JobDef>.GetNamed("VCE_RemoveSoup", true), t);
        }

      
    }
}


