using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;

using Verse;

namespace VanillaPlantsExpanded
{
    public class Plant_RainAffected : Plant
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
                return this.GrowthRateFactor_Fertility * this.GrowthRateFactor_Temperature * this.GrowthRateFactor_Light * this.GrowthRateFactor_Rain;
            }
        }

        public float GrowthRateFactor_Rain
        {
            get
            {
                if (this.Map.weatherManager.RainRate>0)
                {
                    return 1.5f;
                } else return 1f;
            }
        }
    }
}
