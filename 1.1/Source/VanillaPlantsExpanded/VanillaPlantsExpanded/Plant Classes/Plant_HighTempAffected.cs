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
    public class Plant_HighTempAffected : Plant
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
                return this.GrowthRateFactor_Fertility * this.GrowthRateFactor_TemperatureHot * this.GrowthRateFactor_Light;
            }
        }

        public float GrowthRateFactor_TemperatureHot
        {
            get
            {
                float num;
                if (!GenTemperature.TryGetTemperatureForCell(base.Position, base.Map, out num))
                {
                    return 1f;
                }
                if (num < 10f)
                {
                    return Mathf.InverseLerp(0f, 10f, num);
                }
                if (num > 37f && num <60f)
                {
                    return 1.3f;
                }
                if (num > 60f)
                {
                    return Mathf.InverseLerp(70f, 60f, num);
                }
                return 1f;
            }
        }
    }
}
