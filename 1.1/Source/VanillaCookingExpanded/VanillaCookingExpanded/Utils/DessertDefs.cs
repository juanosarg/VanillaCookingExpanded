using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace VanillaCookingExpanded.Utils
{
    [DefOf]
    public static class DessertDefs
    {
        private static List<ThingDef> _desrtDefs;

        public static ThingDef VCE_SimpleDessert;

        public static ThingDef VCE_FineDessert;

        public static ThingDef VCE_LavishDessert;

        public static ThingDef VCE_GourmetDessert;

        public static List<ThingDef> AllDeserts =>
            _desrtDefs ?? (_desrtDefs = new List<ThingDef>()
                {VCE_SimpleDessert, VCE_FineDessert, VCE_LavishDessert, VCE_GourmetDessert});
    }
}
