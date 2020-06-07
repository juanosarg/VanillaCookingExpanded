

using Verse;
using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using System.Diagnostics;
using Verse.Sound;

namespace VanillaCookingExpanded
{
 
    public class Building_ElectricPot : Building, IThingHolder
    {
        private System.Random rand = new System.Random();
        public ThingOwner innerContainerSoup = null;
       
        public Map map;

        public bool ExpectingSoup = false;
      
        public bool StartInsertionJobs = false;

        public string theSoupIAmGoingToInsert = "";

        public string theSoupCooking = "";

        public bool SoupStarted = false;

        protected bool contentsKnown = false;

        public int SoupCounter = 0;
     
        public const int rareTicksPerDay = 240;
        public const int ticksPerDay = 60000;
        public CompPowerTrader compPowerTrader;



        public Building_ElectricPot()
        {
            this.innerContainerSoup = new ThingOwner<Thing>(this, false, LookMode.Deep);
          

        }

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            this.compPowerTrader = base.GetComp<CompPowerTrader>();
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Deep.Look<ThingOwner>(ref this.innerContainerSoup, "innerContainerSoup", new object[]
            {
                this
            });       
            Scribe_Values.Look<string>(ref this.theSoupIAmGoingToInsert, "theSoupIAmGoingToInsert", "", false);
            Scribe_Values.Look<string>(ref this.theSoupCooking, "theSoupCooking", "", false);

            Scribe_Values.Look<bool>(ref this.ExpectingSoup, "ExpectingSoup", false, false);           
            Scribe_Values.Look<bool>(ref this.StartInsertionJobs, "StartInsertionJobs", false, false);
            Scribe_Values.Look<bool>(ref this.contentsKnown, "contentsKnown", false, false);
            Scribe_Values.Look<bool>(ref this.SoupStarted, "SoupStarted", false, false);
            Scribe_Values.Look<int>(ref this.SoupCounter, "SoupCounter", 0, false);





        }

        public ThingOwner GetDirectlyHeldThings()
        {
            return this.innerContainerSoup;
        }

        public void GetChildHolders(List<IThingHolder> outChildren)
        {
            ThingOwnerUtility.AppendThingHoldersFromThings(outChildren, this.GetDirectlyHeldThings());
        }

        public virtual void EjectContentsFirst()
        {
            this.contentsKnown = false;
            base.Map.mapDrawer.MapMeshDirty(base.Position, MapMeshFlag.Things | MapMeshFlag.Buildings);
            this.innerContainerSoup.TryDropAll(this.InteractionCell, base.Map, ThingPlaceMode.Near, null, null);
            this.TickRare();
        }

      

        public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
        {

            EjectContentsFirst();
            base.Destroy(mode);
        }

        public bool TryAcceptSoup(Thing thing, bool allowSpecialEffects = true)
        {
            bool result;

            bool flag;
            if (thing.holdingOwner != null)
            {
                thing.holdingOwner.TryTransferToContainer(thing, this.innerContainerSoup, thing.stackCount, true);
                this.contentsKnown = true;
                base.Map.mapDrawer.MapMeshDirty(base.Position, MapMeshFlag.Things | MapMeshFlag.Buildings);

                flag = true;
            }
            else
            {
                flag = this.innerContainerSoup.TryAdd(thing, true);
                this.contentsKnown = true;
                base.Map.mapDrawer.MapMeshDirty(base.Position, MapMeshFlag.Things | MapMeshFlag.Buildings);
            }
            if (flag)
            {
                if (thing.Faction != null && thing.Faction.IsPlayer)
                {

                    this.contentsKnown = true;
                    base.Map.mapDrawer.MapMeshDirty(base.Position, MapMeshFlag.Things | MapMeshFlag.Buildings);
                }
                result = true;
            }
            else
            {
                result = false;
            }
            this.TickRare();
            return result;
        }

       



        [DebuggerHidden]
        public override IEnumerable<Gizmo> GetGizmos()
        {
            map = this.Map;
            foreach (Gizmo g in base.GetGizmos())
            {
                yield return g;
            }
            if (!SoupStarted)
            {
                yield return SoupListSetupUtility.SetUnfinishedSoupListCommand(this, map);
               
                if (!this.StartInsertionJobs)
                {
                    Command_Action RB_Gizmo_StartInsertion = new Command_Action();
                    RB_Gizmo_StartInsertion.action = delegate
                    {
                        if (ExpectingSoup || (innerContainerSoup.Count > 0))
                        {
                            StartInsertionJobs = true;
                        }
                        else
                        {
                            Messages.Message("VCE_SelectSoup".Translate(), null, MessageTypeDefOf.NegativeEvent, true);
                        }
                    };
                    RB_Gizmo_StartInsertion.defaultLabel = "VCE_StartInsertion".Translate();
                    RB_Gizmo_StartInsertion.defaultDesc = "VCE_StartInsertionDesc".Translate();
                    RB_Gizmo_StartInsertion.icon = ContentFinder<Texture2D>.Get("UI/VCE_InsertSoup", true);
                    yield return RB_Gizmo_StartInsertion;



                    Command_Action RB_Gizmo_RemoveSoup = new Command_Action();
                    RB_Gizmo_RemoveSoup.action = delegate
                    {
                        EjectContentsFirst();
                        ExpectingSoup = false;
                        StartInsertionJobs = false;
                        theSoupIAmGoingToInsert = "";
                    };
                    RB_Gizmo_RemoveSoup.defaultLabel = "VCE_RemoveSoup".Translate();
                    RB_Gizmo_RemoveSoup.defaultDesc = "VCE_RemoveSoupDesc".Translate();
                    RB_Gizmo_RemoveSoup.icon = ContentFinder<Texture2D>.Get("UI/VCE_RemoveSoup", true);
                    yield return RB_Gizmo_RemoveSoup;

                }
                else
                {
                    Command_Action RB_Gizmo_CancelJobs = new Command_Action();
                    RB_Gizmo_CancelJobs.action = delegate
                    {
                        StartInsertionJobs = false;

                    };
                    RB_Gizmo_CancelJobs.defaultLabel = "VCE_CancelBringingSoup".Translate();
                    RB_Gizmo_CancelJobs.defaultDesc = "VCE_CancelBringingSoupDesc".Translate();
                    RB_Gizmo_CancelJobs.icon = ContentFinder<Texture2D>.Get("UI/VCE_CancelSoup", true);
                    yield return RB_Gizmo_CancelJobs;

                    Command_Action RB_Gizmo_Engage = new Command_Action();
                    RB_Gizmo_Engage.action = delegate
                    {
                        if (ExpectingSoup)
                        {
                            Messages.Message("VCE_WaitTillJobsEnd".Translate(), null, MessageTypeDefOf.NegativeEvent, true);
                        }
                        else if (!compPowerTrader.PowerOn)
                        {
                            Messages.Message("VCE_NotPowered".Translate(), null, MessageTypeDefOf.NegativeEvent, true);

                        }
                        else
                        {
                           
                           
                                this.innerContainerSoup.ClearAndDestroyContents();
                                this.contentsKnown = false;
                                theSoupCooking = theSoupIAmGoingToInsert;
                                theSoupIAmGoingToInsert = "";
                                
                                StartInsertionJobs = false;
                                SoupStarted = true;
                            base.Map.mapDrawer.MapMeshDirty(base.Position, MapMeshFlag.Things | MapMeshFlag.Buildings);




                        }

                    };
                    RB_Gizmo_Engage.defaultLabel = "VCE_Engage".Translate();
                    RB_Gizmo_Engage.defaultDesc = "VCE_EngageDesc".Translate();
                    RB_Gizmo_Engage.icon = ContentFinder<Texture2D>.Get("UI/VCE_EngageSoup", true);
                    yield return RB_Gizmo_Engage;
                }

            }


        }

       

        public override void TickRare()
        {
            base.TickRare();
            if (SoupStarted)
            {
                
                SoupCounter++;
                if (!compPowerTrader.PowerOn)
                {
                    Messages.Message("VCE_SoupFailurePower".Translate(), this, MessageTypeDefOf.NegativeEvent, true);
                    SoupCounter = 0;
                    SoupStarted = false;
                    base.Map.mapDrawer.MapMeshDirty(base.Position, MapMeshFlag.Things | MapMeshFlag.Buildings);

                }

                if (SoupCounter > rareTicksPerDay * 3)
                {


                    Messages.Message("VCE_SoupFinished".Translate(), this, MessageTypeDefOf.PositiveEvent, true);


                    base.Map.mapDrawer.MapMeshDirty(base.Position, MapMeshFlag.Things | MapMeshFlag.Buildings);
                    theSoupCooking = "";
                    SoupCounter = 0;

                    SoupStarted = false;
                    
                }

            }

        }

        public override string GetInspectString()
        {


            string text = base.GetInspectString();
            string incubationTxt = "";

            if (SoupStarted)
            {
                incubationTxt = "\n" + "VCE_SoupInProgress".Translate(ThingDef.Named(this.theSoupCooking).LabelCap, ((int)(ticksPerDay * 3) - (SoupCounter * 250)).ToStringTicksToPeriod(true, false, true, true));
            }


            return text + incubationTxt;
        }

        public override Graphic Graphic
        {
            get
            {
                if (contentsKnown || SoupStarted)
                {
                   
                    Graphic newgraphic = GraphicDatabase.Get(typeof(Graphic_Multi), "Things/Buildings/VCE_PotFull", this.def.graphicData.shaderType.Shader, this.def.graphicData.drawSize, this.DrawColor, this.DrawColorTwo);

                    return newgraphic;
                }
                else

                {
                    return this.DefaultGraphic;

                }




            }
        }

    }
}
