using System;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;

namespace AchievementsExpanded
{
    public class HediffWithSeverityTracker:HediffTracker 
    {
        
        public HediffWithSeverityTracker()
        {
        }

        public HediffWithSeverityTracker(HediffWithSeverityTracker reference) : base(reference)
        {
            severity = reference.severity;
            
        }

        public override void ExposeData()
        {
            base.ExposeData();
           
            Scribe_Values.Look(ref severity, "severity");
           
        }

        public override bool Trigger(Hediff hediff)
        {
           
           

            if (hediff?.pawn != null && hediff.pawn.Faction == Faction.OfPlayer && (def is null || def == hediff.def)
                && hediff.pawn.health.hediffSet.GetFirstHediffOfDef(hediff.def).Severity >= severity)
            {
                triggeredCount++;
            }
            return triggeredCount >= count;
        }

      
        public float severity;

        private int triggeredCount;
    }
}
