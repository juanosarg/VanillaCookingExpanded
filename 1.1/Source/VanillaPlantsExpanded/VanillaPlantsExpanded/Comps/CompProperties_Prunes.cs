using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using RimWorld;

namespace VanillaPlantsExpanded
{
    public class CompProperties_Prunes : CompProperties
    {
        public int TicksToRotStart
        {
            get
            {
                return Mathf.RoundToInt(this.daysToRotStart * 60000f);
            }
        }

        public int TicksToDessicated
        {
            get
            {
                return Mathf.RoundToInt(this.daysToDessicated * 60000f);
            }
        }

        public CompProperties_Prunes()
        {
            this.compClass = typeof(CompPrunes);
        }

        public CompProperties_Prunes(float daysToRotStart)
        {
            this.daysToRotStart = daysToRotStart;
        }

       

        public float daysToRotStart = 2f;

        public bool rotDestroys;

        public float rotDamagePerDay = 40f;

        public float daysToDessicated = 999f;

        public float dessicatedDamagePerDay;

        public bool disableIfHatcher;
    }
}
