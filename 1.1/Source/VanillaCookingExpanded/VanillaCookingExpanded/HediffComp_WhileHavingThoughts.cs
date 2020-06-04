

using RimWorld;
using System.Collections.Generic;
using Verse;
using System.Linq;

namespace VanillaCookingExpanded
{
    class HediffComp_WhileHavingThoughts : HediffComp
    {
        public bool flagAmIThinking = false;

        public int checkingInterval = 600;

        public int checkingCounter = 0;

        public override void CompExposeData()
        {
            Scribe_Values.Look<bool>(ref this.flagAmIThinking, "flagAmIThinking", false, false);
        }

        public HediffCompProperties_WhileHavingThoughts Props
        {
            get
            {
                return (HediffCompProperties_WhileHavingThoughts)this.props;
            }
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);

            checkingCounter++;

            if (checkingCounter > checkingInterval)
            {
                foreach (ThoughtDef thoughtDef in this.Props.thoughtDefs)
                {
                    flagAmIThinking = false;
                    if (this.Pawn.needs.mood.thoughts.memories.GetFirstMemoryOfDef(thoughtDef) != null)
                    {
                        flagAmIThinking = true;
                        break;
                    }
                }

                if (!flagAmIThinking)
                {
                    this.Pawn.health.RemoveHediff(this.parent);
                }
                checkingCounter = 0;
            }


        }

    }
}
