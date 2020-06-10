

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

    public class Building_CheesePress : Building, IThingHolder
    {
        private System.Random rand = new System.Random();
        public ThingOwner innerContainerMilk = null;

        public Map map;

        public bool ExpectingMilk = false;

        public bool CheeseReadyAndWaitingForPickup = false;

        public bool YouCanNowRemoveAverageQualityCheese = false;

        public bool StartInsertionJobs = false;

        public string theMilkIAmGoingToInsert = "";

        public string theMilkCooking = "";

        public string ingredients = "";

        public bool CheeseStarted = false;

        protected bool contentsKnown = false;

        public int CheeseCounter = 0;

        public const int rareTicksPerDay = 240;
        public const int ticksPerDay = 60000;


        public string cheeseToTurnInto = "";
        public int amount = 0;
     
        public float awfulQualityAgeDaysThreshold;
        public float poorQualityAgeDaysThreshold;
		public float normalQualityAgeDaysThreshold;
        public float goodQualityAgeDaysThreshold;
        public float excellentQualityAgeDaysThreshold;
        public float masterworkQualityAgeDaysThreshold;
        public float legendaryQualityAgeDaysThreshold;

        public QualityCategory qualityNow = QualityCategory.Awful;



        public Building_CheesePress()
        {
            this.innerContainerMilk = new ThingOwner<Thing>(this, false, LookMode.Deep);


        }

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);

        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Deep.Look<ThingOwner>(ref this.innerContainerMilk, "innerContainerMilk", new object[]
            {
                this
            });
            Scribe_Values.Look(ref qualityNow, "qualityNow", QualityCategory.Awful);
            Scribe_Values.Look<string>(ref this.theMilkIAmGoingToInsert, "theMilkIAmGoingToInsert", "", false);
            Scribe_Values.Look<string>(ref this.theMilkCooking, "theMilkCooking", "", false);
            Scribe_Values.Look<bool>(ref this.ExpectingMilk, "ExpectingMilk", false, false);
            Scribe_Values.Look<bool>(ref this.StartInsertionJobs, "StartInsertionJobs", false, false);
            Scribe_Values.Look<bool>(ref this.contentsKnown, "contentsKnown", false, false);
            Scribe_Values.Look<bool>(ref this.CheeseStarted, "CheeseStarted", false, false);
            Scribe_Values.Look<bool>(ref this.CheeseReadyAndWaitingForPickup, "CheeseReadyAndWaitingForPickup", false, false);
            Scribe_Values.Look<int>(ref this.CheeseCounter, "SoupCounter", 0, false);
            Scribe_Values.Look<string>(ref this.cheeseToTurnInto, "cheeseToTurnInto", "", false);
            Scribe_Values.Look<int>(ref this.amount, "amount", 0, false);
            Scribe_Values.Look<bool>(ref this.YouCanNowRemoveAverageQualityCheese, "YouCanNowRemoveAverageQualityCheese", false, false);
            Scribe_Values.Look<string>(ref this.ingredients, "ingredients", "", false);

            Scribe_Values.Look<float>(ref this.awfulQualityAgeDaysThreshold, "awfulQualityAgeDaysThreshold", 0f, false);
            Scribe_Values.Look<float>(ref this.poorQualityAgeDaysThreshold, "poorQualityAgeDaysThreshold", 0f, false);
            Scribe_Values.Look<float>(ref this.normalQualityAgeDaysThreshold, "normalQualityAgeDaysThreshold", 0f, false);
            Scribe_Values.Look<float>(ref this.goodQualityAgeDaysThreshold, "goodQualityAgeDaysThreshold", 0f, false);
            Scribe_Values.Look<float>(ref this.excellentQualityAgeDaysThreshold, "excellentQualityAgeDaysThreshold", 0f, false);
            Scribe_Values.Look<float>(ref this.masterworkQualityAgeDaysThreshold, "masterworkQualityAgeDaysThreshold", 0f, false);
            Scribe_Values.Look<float>(ref this.legendaryQualityAgeDaysThreshold, "legendaryQualityAgeDaysThreshold", 0f, false);

        }

        public ThingOwner GetDirectlyHeldThings()
        {
            return this.innerContainerMilk;
        }

        public void GetChildHolders(List<IThingHolder> outChildren)
        {
            ThingOwnerUtility.AppendThingHoldersFromThings(outChildren, this.GetDirectlyHeldThings());
        }

        public virtual void EjectContentsFirst()
        {
            this.contentsKnown = false;
            base.Map.mapDrawer.MapMeshDirty(base.Position, MapMeshFlag.Things | MapMeshFlag.Buildings);
            this.innerContainerMilk.TryDropAll(this.InteractionCell, base.Map, ThingPlaceMode.Near, null, null);
            this.TickRare();
        }



        public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
        {

            EjectContentsFirst();
            base.Destroy(mode);
        }

        public bool TryAcceptMilk(Thing thing, bool allowSpecialEffects = true)
        {
            bool result;

            bool flag;
            if (thing.holdingOwner != null)
            {
                thing.holdingOwner.TryTransferToContainer(thing, this.innerContainerMilk, thing.stackCount, true);
                this.contentsKnown = true;
                base.Map.mapDrawer.MapMeshDirty(base.Position, MapMeshFlag.Things | MapMeshFlag.Buildings);

                flag = true;
            }
            else
            {
                flag = this.innerContainerMilk.TryAdd(thing, true);
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
            if (!CheeseStarted && !CheeseReadyAndWaitingForPickup)
            {
                yield return MilkListSetupUtility.SetMilkListCommand(this, map);

                if (!this.StartInsertionJobs)
                {
                    Command_Action RB_Gizmo_StartInsertion = new Command_Action();
                    RB_Gizmo_StartInsertion.action = delegate
                    {
                        if (ExpectingMilk || (innerContainerMilk.Count > 0))
                        {
                            StartInsertionJobs = true;
                        }
                        else
                        {
                            Messages.Message("VCE_SelectMilk".Translate(), null, MessageTypeDefOf.NegativeEvent, true);
                        }
                    };
                    RB_Gizmo_StartInsertion.defaultLabel = "VCE_StartInsertionMilk".Translate();
                    RB_Gizmo_StartInsertion.defaultDesc = "VCE_StartInsertionMilkDesc".Translate();
                    RB_Gizmo_StartInsertion.icon = ContentFinder<Texture2D>.Get("UI/VCE_InsertMilk", true);
                    yield return RB_Gizmo_StartInsertion;



                    Command_Action RB_Gizmo_RemoveSoup = new Command_Action();
                    RB_Gizmo_RemoveSoup.action = delegate
                    {
                        EjectContentsFirst();
                        ExpectingMilk = false;
                        StartInsertionJobs = false;
                        theMilkIAmGoingToInsert = "";
                    };
                    RB_Gizmo_RemoveSoup.defaultLabel = "VCE_RemoveMilk".Translate();
                    RB_Gizmo_RemoveSoup.defaultDesc = "VCE_RemoveMilkDesc".Translate();
                    RB_Gizmo_RemoveSoup.icon = ContentFinder<Texture2D>.Get("UI/VCE_RemoveMilk", true);
                    yield return RB_Gizmo_RemoveSoup;

                }
                else
                {
                    Command_Action RB_Gizmo_CancelJobs = new Command_Action();
                    RB_Gizmo_CancelJobs.action = delegate
                    {
                        StartInsertionJobs = false;

                    };
                    RB_Gizmo_CancelJobs.defaultLabel = "VCE_CancelBringingMilk".Translate();
                    RB_Gizmo_CancelJobs.defaultDesc = "VCE_CancelBringingMilkDesc".Translate();
                    RB_Gizmo_CancelJobs.icon = ContentFinder<Texture2D>.Get("UI/VCE_CancelMilk", true);
                    yield return RB_Gizmo_CancelJobs;

                    Command_Action RB_Gizmo_Engage = new Command_Action();
                    RB_Gizmo_Engage.action = delegate
                    {
                        if (ExpectingMilk)
                        {
                            Messages.Message("VCE_WaitTillJobsEndMilk".Translate(), null, MessageTypeDefOf.NegativeEvent, true);
                        }

                        else
                        {
                            if (this.innerContainerMilk.First().def.HasModExtension<Milk_Extension>())
                            {
                                Milk_Extension thisExtension = this.innerContainerMilk.First().def.GetModExtension<Milk_Extension>();
                                this.cheeseToTurnInto = thisExtension.cheeseToTurnInto;
                                this.amount = thisExtension.amount;
                               
                                this.innerContainerMilk.ClearAndDestroyContents();
                                this.contentsKnown = false;
                                this.awfulQualityAgeDaysThreshold = thisExtension.awfulQualityAgeDaysThreshold;
                                this.poorQualityAgeDaysThreshold = thisExtension.poorQualityAgeDaysThreshold;
                                this.normalQualityAgeDaysThreshold = thisExtension.normalQualityAgeDaysThreshold;
                                this.goodQualityAgeDaysThreshold = thisExtension.goodQualityAgeDaysThreshold;
                                this.excellentQualityAgeDaysThreshold = thisExtension.excellentQualityAgeDaysThreshold;
                                this.masterworkQualityAgeDaysThreshold = thisExtension.masterworkQualityAgeDaysThreshold;
                                this.legendaryQualityAgeDaysThreshold = thisExtension.legendaryQualityAgeDaysThreshold;
                               

                                theMilkCooking = theMilkIAmGoingToInsert;
                                ingredients = theMilkCooking;
                                theMilkIAmGoingToInsert = "";
                                StartInsertionJobs = false;
                                CheeseStarted = true;
                                base.Map.mapDrawer.MapMeshDirty(base.Position, MapMeshFlag.Things | MapMeshFlag.Buildings);

                            }
                            else
                            {
                                Messages.Message("VCE_MilkImproperlyDefined".Translate(), null, MessageTypeDefOf.NegativeEvent, true);
                            }





                        }

                    };
                    RB_Gizmo_Engage.defaultLabel = "VCE_EngageMilk".Translate();
                    RB_Gizmo_Engage.defaultDesc = "VCE_EngageMilkDesc".Translate();
                    RB_Gizmo_Engage.icon = ContentFinder<Texture2D>.Get("UI/VCE_EngageMilk", true);
                    yield return RB_Gizmo_Engage;
                }

            }
            if (CheeseStarted && YouCanNowRemoveAverageQualityCheese)
            {
                Command_Action RB_Gizmo_RemoveCheese = new Command_Action();
                RB_Gizmo_RemoveCheese.action = delegate
                {
                   
                    CheeseReadyAndWaitingForPickup = true;
                    base.Map.mapDrawer.MapMeshDirty(base.Position, MapMeshFlag.Things | MapMeshFlag.Buildings);
                    theMilkCooking = "";
                    CheeseCounter = 0;
                    CheeseStarted = false;
                };
                RB_Gizmo_RemoveCheese.defaultLabel = "VCE_RemoveCheese".Translate(qualityNow.ToString());
                RB_Gizmo_RemoveCheese.defaultDesc = "VCE_RemoveCheeseDesc".Translate(qualityNow.ToString());
                RB_Gizmo_RemoveCheese.icon = ContentFinder<Texture2D>.Get("UI/VCE_RemoveCheese", true);
                yield return RB_Gizmo_RemoveCheese;
            }


        }

      

        public override void TickRare()
        {
            base.TickRare();
            if (CheeseStarted)
            {

                CheeseCounter++;


                if (CheeseCounter > rareTicksPerDay * this.legendaryQualityAgeDaysThreshold)
                {

                    qualityNow = QualityCategory.Legendary;
                    Messages.Message("VCE_CheeseFinished".Translate(), this, MessageTypeDefOf.PositiveEvent, true);
                    base.Map.mapDrawer.MapMeshDirty(base.Position, MapMeshFlag.Things | MapMeshFlag.Buildings);
                    CheeseReadyAndWaitingForPickup = true;
                    theMilkCooking = "";
                    CheeseCounter = 0;

                    CheeseStarted = false;

                }
                else if (CheeseCounter == rareTicksPerDay * this.awfulQualityAgeDaysThreshold)
                {
                    qualityNow = QualityCategory.Awful;
                    YouCanNowRemoveAverageQualityCheese = true;
                }
                else if (CheeseCounter == rareTicksPerDay * this.poorQualityAgeDaysThreshold)
                {
                    qualityNow = QualityCategory.Poor;
                }
                else if (CheeseCounter == rareTicksPerDay * this.normalQualityAgeDaysThreshold)
                {
                    qualityNow = QualityCategory.Normal;
                }
                else if (CheeseCounter == rareTicksPerDay * this.goodQualityAgeDaysThreshold)
                {
                    qualityNow = QualityCategory.Good;
                }
                else if (CheeseCounter == rareTicksPerDay * this.excellentQualityAgeDaysThreshold)
                {
                    qualityNow = QualityCategory.Excellent;
                }
                else if (CheeseCounter == rareTicksPerDay * this.masterworkQualityAgeDaysThreshold)
                {
                    qualityNow = QualityCategory.Masterwork;
                }
            }

        }

        public override string GetInspectString()
        {


            string text = base.GetInspectString();
            string incubationTxt = "";

            if (!contentsKnown && !CheeseStarted)
            {
                incubationTxt += "VCE_CheesePressEmpty".Translate();

            }
            else if(contentsKnown && !CheeseStarted)
            {
                incubationTxt += "VCE_CheesePressFilled".Translate(this.innerContainerMilk.First().def.LabelCap);

            } else


            if (CheeseStarted) {
                incubationTxt += "VCE_CheesePressWorking".Translate();
                if (!YouCanNowRemoveAverageQualityCheese)
                {
                    incubationTxt += "\n" + "VCE_CheeseInProgress".Translate(ThingDef.Named(this.theMilkCooking).LabelCap, qualityNow.ToString(),((int)(ticksPerDay * this.awfulQualityAgeDaysThreshold) - (CheeseCounter * 250)).ToStringTicksToPeriod(true, false, true, true));
                }
                else {

                    switch (qualityNow)
                    {
                        case QualityCategory.Awful:
                            incubationTxt += "\n" + "VCE_CheeseInProgress".Translate(ThingDef.Named(this.theMilkCooking).LabelCap, QualityCategory.Poor.ToString(), ((int)(ticksPerDay * this.poorQualityAgeDaysThreshold) - (CheeseCounter * 250)).ToStringTicksToPeriod(true, false, true, true));
                            break;
                        case QualityCategory.Poor:
                            incubationTxt += "\n" + "VCE_CheeseInProgress".Translate(ThingDef.Named(this.theMilkCooking).LabelCap, QualityCategory.Normal.ToString(), ((int)(ticksPerDay * this.normalQualityAgeDaysThreshold) - (CheeseCounter * 250)).ToStringTicksToPeriod(true, false, true, true));
                            break;
                        case QualityCategory.Normal:
                            incubationTxt += "\n" + "VCE_CheeseInProgress".Translate(ThingDef.Named(this.theMilkCooking).LabelCap, QualityCategory.Good.ToString(), ((int)(ticksPerDay * this.goodQualityAgeDaysThreshold) - (CheeseCounter * 250)).ToStringTicksToPeriod(true, false, true, true));
                            break;
                        case QualityCategory.Good:
                            incubationTxt += "\n" + "VCE_CheeseInProgress".Translate(ThingDef.Named(this.theMilkCooking).LabelCap, QualityCategory.Excellent.ToString(), ((int)(ticksPerDay * this.excellentQualityAgeDaysThreshold) - (CheeseCounter * 250)).ToStringTicksToPeriod(true, false, true, true));
                            break;
                        case QualityCategory.Excellent:
                            incubationTxt += "\n" + "VCE_CheeseInProgress".Translate(ThingDef.Named(this.theMilkCooking).LabelCap, QualityCategory.Masterwork.ToString(), ((int)(ticksPerDay * this.masterworkQualityAgeDaysThreshold) - (CheeseCounter * 250)).ToStringTicksToPeriod(true, false, true, true));
                            break;
                        case QualityCategory.Masterwork:
                            incubationTxt += "\n" + "VCE_CheeseInProgress".Translate(ThingDef.Named(this.theMilkCooking).LabelCap, QualityCategory.Legendary.ToString(), ((int)(ticksPerDay * this.legendaryQualityAgeDaysThreshold) - (CheeseCounter * 250)).ToStringTicksToPeriod(true, false, true, true));
                            break;


                    }

                }
                

            }
            

            if (CheeseReadyAndWaitingForPickup)
            {
                incubationTxt += "\n" + "VCE_CheeseReady".Translate();
            }


            return text + incubationTxt;
        }

        public override Graphic Graphic
        {
            get
            {
                if (contentsKnown || CheeseStarted)
                {

                    Graphic newgraphic = GraphicDatabase.Get(typeof(Graphic_Single), "Things/Buildings/VCE_CheesePressFull", this.def.graphicData.shaderType.Shader, this.def.graphicData.drawSize, this.DrawColor, this.DrawColorTwo);

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
