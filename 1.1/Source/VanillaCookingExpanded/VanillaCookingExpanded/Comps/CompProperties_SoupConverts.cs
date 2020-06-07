using RimWorld;
using Verse;

namespace VanillaCookingExpanded
{
    public class CompProperties_SoupConverts : CompProperties
    {

        public string soupToTurnInto = "VCE_CookedSoupSimple";
        public int amount = 10;
        public int days = 3;

        public CompProperties_SoupConverts()
        {
            this.compClass = typeof(CompSoupConverts);
        }
    }
}
