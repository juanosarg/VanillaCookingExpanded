using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using HarmonyLib;
using Verse;
using RimWorld;
using RimWorld.Planet;

namespace AchievementsExpanded
{
    public class Plant_PlantCollected_Patch
    {

        public static void CheckPlantHarvested(Plant __instance)
        {
            if (__instance is Plant plant && Current.ProgramState == ProgramState.Playing)
            {
                foreach (var card in AchievementPointManager.GetCards<PlantTracker>())
                {
                    if ((card.tracker as PlantTracker).Trigger(plant))
                    {
                        card.UnlockCard();
                    }
                }
            }
        }
    }
}
