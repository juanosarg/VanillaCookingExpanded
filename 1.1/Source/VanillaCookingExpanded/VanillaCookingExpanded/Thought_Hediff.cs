


using Verse;
using System.Collections.Generic;
using System.Linq;
using RimWorld;

namespace VanillaCookingExpanded
{
    class Thought_Hediff : Thought_Memory
    {
        public bool added = false;

        public override float MoodOffset()
        {
            if (!added)
            {
                this.pawn.health.AddHediff(this.def.hediff);
                added = true;
            }

            return base.MoodOffset();
        }


    }
}
