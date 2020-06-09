

using RimWorld;
using System.Collections.Generic;
using Verse;
using System.Text;

namespace VanillaCookingExpanded
{
    class HediffCompProperties_WhileHavingThoughts : HediffCompProperties
    {

        public List<ThoughtDef> thoughtDefs;
        public bool resurrectionEffect = false;

        public HediffCompProperties_WhileHavingThoughts()
        {
            this.compClass = typeof(HediffComp_WhileHavingThoughts);
        }
    }
}
