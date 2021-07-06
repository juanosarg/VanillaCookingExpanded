using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;

namespace VanillaCookingExpanded
{
    [DefOf]
    public static class ThoughtDefs
    {
        private static List<ThoughtDef> _thoughtDefs;

        public static ThoughtDef VCE_ConsumedSugar;
        public static ThoughtDef VCE_ConsumedChocolateSyrup;
        public static ThoughtDef VCE_ConsumedInsectJelly;
        public static ThoughtDef VCE_SmokeleafButterHigh;
        public static ThoughtDef VCE_ConsumedSalt;
        public static ThoughtDef VCE_ConsumedMayo;
        public static ThoughtDef VCE_ConsumedAgave;
        public static ThoughtDef VCE_ConsumedSpices;
        public static ThoughtDef VCE_ConsumedDigestibleResurrectorNanites;
        public static ThoughtDef VCE_AteFriedGoods;
        public static ThoughtDef VCE_ConsumedCannedGoods;

        public static List<ThoughtDef> AllThoughts =>
            _thoughtDefs ?? (_thoughtDefs = new List<ThoughtDef>()
            {
                VCE_ConsumedSugar,
                VCE_ConsumedChocolateSyrup,
                VCE_ConsumedInsectJelly,
                VCE_SmokeleafButterHigh,
                VCE_ConsumedSalt,
                VCE_ConsumedMayo,
                VCE_ConsumedAgave,
                VCE_ConsumedSpices,
                VCE_ConsumedDigestibleResurrectorNanites,
                VCE_AteFriedGoods,
                VCE_ConsumedCannedGoods,
            });
    }
}
