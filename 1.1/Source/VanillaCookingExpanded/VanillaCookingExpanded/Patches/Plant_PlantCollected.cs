using HarmonyLib;
using RimWorld;
using System.Reflection;
using Verse;
using System.Collections.Generic;
using RimWorld.Planet;
using System.Linq;
using System;

namespace VanillaCookingExpanded
{
   
    [HarmonyPatch(typeof(Plant))]
    [HarmonyPatch("PlantCollected")]
    public static class VanillaCookingExpanded_Plant_PlantCollected_Patch
    {
        [HarmonyPrefix]
        public static void RemoveTilled(ref Plant __instance)
        {

            if (__instance.Map.terrainGrid.TerrainAt(__instance.Position).defName== "VCE_TilledSoil")

            {
                __instance.Map.terrainGrid.RemoveTopLayer(__instance.Position);

            }
           



        }


    }

}
