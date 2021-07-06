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
    public class ItemTrackerWithQuality : ItemTracker
    {
       

        public ItemTrackerWithQuality()
        { 
        }

        public ItemTrackerWithQuality(ItemTrackerWithQuality reference) : base(reference)
        {
            quality = reference.quality;
          
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref quality, "quality", QualityCategory.Awful);

        }

        public override bool Trigger()
        {
            base.Trigger();
            return PlayerHasWithQuality(def, quality, out int _, count);
        }

        public override bool UnlockOnStartup => Trigger();

        public QualityCategory quality;

        public static bool PlayerHasWithQuality(ThingDef thingDef, QualityCategory quality,out int total, int count = 1)
        {
            if (count <= 0)
            {
                total = 0;
                return true;
            }
            int num = 0;
            List<Map> maps = Find.Maps;
            for (int i = 0; i < maps.Count; i++)
            {

                List<Thing> thingsList = maps[i].listerThings.ThingsOfDef(thingDef);
                foreach (Thing thing in thingsList)
                {
                    //Log.Message("Cheese found");
                    if (thing.TryGetComp<CompQuality>() is CompQuality qualityComp &&
                        qualityComp.Quality == quality)
                    {
                        //Log.Message("Quality is "+ qualityComp.Quality);
                        num += thing.stackCount;
                    }
                }
                
                if (num >= count)
                {
                    total = num;
                    return true;
                }
            }
            total = num;
            List<Caravan> caravans = Find.WorldObjects.Caravans;
            for (int j = 0; j < caravans.Count; j++)
            {
                if (caravans[j].IsPlayerControlled)
                {
                    List<Thing> list = CaravanInventoryUtility.AllInventoryItems(caravans[j]);
                    for (int k = 0; k < list.Count; k++)
                    {
                        if (list[k].def == thingDef)
                        {
                            if (list[k].TryGetComp<CompQuality>() is CompQuality qualityComp &&
                        qualityComp.Quality == quality)
                            {
                                num += list[k].stackCount;
                                total += num;
                            }
                                
                            if (num >= count)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return total >= count;
        }

    }
}
