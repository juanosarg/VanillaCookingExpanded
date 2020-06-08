using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse.AI;
using Verse;
using System.Linq;


namespace VanillaCookingExpanded
{
    [StaticConstructorOnStartup]
    public class Command_MilkList : Command
    {

        public Map map;
        public Building_CheesePress building;
        public List<Thing> milk;



        public Command_MilkList()
        {
            foreach (object obj in Find.Selector.SelectedObjects)
            {
                building = obj as Building_CheesePress;

               

                if (building != null)
                {
                    if (building.theMilkIAmGoingToInsert == "")
                    {
                        icon = ContentFinder<Texture2D>.Get("UI/VCE_EmptyMilkIcon", true);
                        defaultLabel = "VCE_InsertMilk".Translate();
                    }

                    foreach (AcceptedMilksDef element in DefDatabase<AcceptedMilksDef>.AllDefs)
                    {
                        foreach (string milkNow in element.milks)
                        {
                            if (building.theMilkIAmGoingToInsert == milkNow)
                            {
                                icon = ContentFinder<Texture2D>.Get(ThingDef.Named(milkNow).graphic.path, true);
                                defaultLabel = "VCE_InsertMilkVariable".Translate(ThingDef.Named(milkNow).LabelCap);
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

            foreach (AcceptedMilksDef element in DefDatabase<AcceptedMilksDef>.AllDefs)
            {
                foreach (string thisMilk in element.milks)
                {

                    list.Add(new FloatMenuOption("VCE_InsertMilkVariable".Translate(ThingDef.Named(thisMilk).LabelCap), delegate
                    {
                        milk = map.listerThings.ThingsOfDef(DefDatabase<ThingDef>.GetNamed(thisMilk, true));
                        if (milk.Count > 0)
                        {

                            this.TryInsertMilk();
                        }
                        else
                        {
                            Messages.Message("VCE_NoMilksFound".Translate(), null, MessageTypeDefOf.NegativeEvent, true);
                        }

                    }, MenuOptionPriority.Default, null, null, 29f, null, null));

                }
            }
            Find.WindowStack.Add(new FloatMenu(list));
        }

        private void TryInsertMilk()
        {
            Building_CheesePress building = (Building_CheesePress)this.building;
            //Log.Message("Inserting "+ genome.RandomElement().def.defName +" on "+building.ToString());
            building.ExpectingMilk = true;
            building.theMilkIAmGoingToInsert = milk.RandomElement().def.defName;
        }




    }


   

}

