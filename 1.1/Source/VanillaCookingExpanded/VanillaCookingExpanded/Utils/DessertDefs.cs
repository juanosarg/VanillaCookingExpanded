using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace VanillaCookingExpanded
{
    [DefOf]
    public static class DessertDefs
    {
        private static HashSet<ThingDef> _desrtDefs;

        public static ThingDef VCE_SimpleDessert;

        public static ThingDef VCE_FineDessert;

        public static ThingDef VCE_LavishDessert;

        public static ThingDef VCE_GourmetDessert;

        public static HashSet<ThingDef> AllDeserts =>
            _desrtDefs ?? (_desrtDefs = new HashSet<ThingDef>()
                {VCE_SimpleDessert, VCE_FineDessert, VCE_LavishDessert, VCE_GourmetDessert});
    }
}
