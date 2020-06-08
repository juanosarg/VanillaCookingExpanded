using RimWorld;
using Verse;

using UnityEngine;


namespace VanillaCookingExpanded
{
    public static class SoupListSetupUtility
    {
        public static Command_UnfinishedSoupList SetUnfinishedSoupListCommand(Building_ElectricPot passingbuilding, Map passingMap)
        {
            return new Command_UnfinishedSoupList()
            {
                defaultDesc = "VCE_InsertSoupDesc".Translate(),
                hotKey = KeyBindingDefOf.Misc1,
                map = passingMap,
                building = passingbuilding

            };
        }

       
    }
}
