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
    public class Plant_ChecksRiver: Plant
    {

        const int radius = 3;
        int numberOfRiver = 0;


        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            int num = GenRadial.NumCellsInRadius(radius);
            for (int i = 0; i < num; i++)
            {
                IntVec3 c = this.Position + GenRadial.RadialPattern[i];
                TerrainDef terrain = c.GetTerrain(map);
               
                if (terrain != null && terrain.IsRiver)
                {
                    numberOfRiver++;
                }


              
            }

        }

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
                return this.GrowthRateFactor_Fertility * this.GrowthRateFactor_Temperature * this.GrowthRateFactor_Light * this.GrowthRateFactor_River;
            }
        }

        public float GrowthRateFactor_River
        {
            get
            {
               
                return 1f + (0.01f* numberOfRiver);
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            
            Scribe_Values.Look<int>(ref this.numberOfRiver, "numberOfRiver", 0, false);
       
        }


    }
}
