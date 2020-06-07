using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse.AI;
using Verse;
using System.Linq;


namespace VanillaCookingExpanded
{
    [StaticConstructorOnStartup]
    public class Command_UnfinishedSoupList : Command
    {

        public Map map;
        public Building_ElectricPot building;
        public List<Thing> soup;



        public Command_UnfinishedSoupList()
        {
            foreach (object obj in Find.Selector.SelectedObjects)
            {
                building = obj as Building_ElectricPot;

               

                if (building != null)
                {
                    if (building.theSoupIAmGoingToInsert == "")
                    {
                        icon = ContentFinder<Texture2D>.Get("UI/VCE_EmptySoupPotIcon", true);
                        defaultLabel = "VCE_InsertSoup".Translate();
                    }

                    foreach (AcceptedSoupsDef element in DefDatabase<AcceptedSoupsDef>.AllDefs)
                    {
                        foreach (string soup in element.soups)
                        {
                            if (building.theSoupIAmGoingToInsert == soup)
                            {
                                icon = ContentFinder<Texture2D>.Get("Things/Items/Uncookedsoup/" + soup, true);
                                defaultLabel = (soup + "_InsertSoup").Translate();
                            }
                        }
                    }
                }
            }
        }

        public override void ProcessInput(Event ev)
        {
            base.ProcessInput(ev);
            List<FloatMenuOption> list = new List<FloatMenuOption>();

            foreach (AcceptedSoupsDef element in DefDatabase<AcceptedSoupsDef>.AllDefs)
            {
                foreach (string thisSoup in element.soups)
                {

                    list.Add(new FloatMenuOption(thisSoup.Translate(), delegate
                    {
                        soup = map.listerThings.ThingsOfDef(DefDatabase<ThingDef>.GetNamed(thisSoup, true));
                        if (soup.Count > 0)
                        {

                            this.TryInsertSoup();
                        }
                        else
                        {
                            Messages.Message("VCE_NoSoupsFound".Translate(), null, MessageTypeDefOf.NegativeEvent, true);
                        }

                    }, MenuOptionPriority.Default, null, null, 29f, null, null));

                }
            }
            Find.WindowStack.Add(new FloatMenu(list));
        }

        private void TryInsertSoup()
        {
            Building_ElectricPot building = (Building_ElectricPot)this.building;
            //Log.Message("Inserting "+ genome.RandomElement().def.defName +" on "+building.ToString());
            building.ExpectingSoup = true;
            building.theSoupIAmGoingToInsert = soup.RandomElement().def.defName;
        }




    }


   

}

