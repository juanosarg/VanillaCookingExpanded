using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using UnityEngine;

using Verse;

namespace VanillaPlantsExpanded
{
    public class Plant_StopsGrowingInToxins : Plant
    {

        public override float GrowthRate
        {
            get
            {
                if (this.Blighted)
                {
                    return 0f;
                }
                if (base.Spawned && !PlantUtility.GrowthSeasonNow(base.Position, base.Map, false))
                {
                    return 0f;
                }
                return this.GrowthRateFactor_Fertility * this.GrowthRateFactor_Temperature * this.GrowthRateFactor_Light * this.GrowthRateFactor_Toxins;
            }
        }

        public float GrowthRateFactor_Toxins
        {
            get
            {
                if (this.Map.gameConditionManager.GetActiveCondition(GameConditionDefOf.ToxicFallout)!=null)
                {
                    return 0f;
                }
                else return 1f;
            }
        }
    }
}
