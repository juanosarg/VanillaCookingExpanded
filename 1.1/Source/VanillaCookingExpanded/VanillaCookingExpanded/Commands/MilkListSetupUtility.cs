using RimWorld;
using Verse;

using UnityEngine;


namespace VanillaCookingExpanded
{
    public static class MilkListSetupUtility
    {
        public static Command_MilkList SetMilkListCommand(Building_CheesePress passingbuilding, Map passingMap)
        {
            return new Command_MilkList()
            {
                defaultDesc = "VCE_InsertMilkDesc".Translate(),
                hotKey = KeyBindingDefOf.Misc1,
                map = passingMap,
                building = passingbuilding

            };
        }

       
    }
}
