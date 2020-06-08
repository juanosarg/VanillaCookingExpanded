using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;
using Verse.AI;
using RimWorld;

namespace VanillaCookingExpanded
{
    public class JobDriver_RemoveCheese : JobDriver
    {
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return this.pawn.Reserve(this.job.targetA, this.job, 1, -1, null);
        }

        public override void Notify_PatherFailed()
        {

            Building_CheesePress building_press = (Building_CheesePress)this.job.GetTarget(TargetIndex.A).Thing;

            this.EndJobWith(JobCondition.ErroredPather);

        }

        [DebuggerHidden]
        protected override IEnumerable<Toil> MakeNewToils()
        {
            //Log.Message("I am inside the job now, with "+pawn.ToString(), false);
            Building_CheesePress building_press = (Building_CheesePress)this.job.GetTarget(TargetIndex.A).Thing;
            this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
            this.FailOnBurningImmobile(TargetIndex.A);
            yield return Toils_General.DoAtomic(delegate
            {
                this.job.count =1;
            });
           
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
            yield return Toils_General.Wait(240).FailOnDestroyedNullOrForbidden(TargetIndex.A).FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch).WithProgressBarToilDelay(TargetIndex.A, false, -0.5f);
            yield return new Toil
            {
                initAction = delegate
                {

                    building_press.CheeseReadyAndWaitingForPickup = false;
                    Thing newCheese = ThingMaker.MakeThing(ThingDef.Named(building_press.cheeseToTurnInto));
                    newCheese.stackCount = building_press.amount;
                    newCheese.TryGetComp<CompIngredients>().ingredients.Add(ThingDef.Named(building_press.ingredients));
                    if (newCheese.TryGetComp<CompQuality>() is CompQuality qualityComp)
                    {
                        qualityComp.SetQuality(building_press.qualityNow, ArtGenerationContext.Colony);
                    }
                    building_press.cheeseToTurnInto = "";
                    building_press.ingredients = "";
                    building_press.YouCanNowRemoveAverageQualityCheese = false;
                    building_press.qualityNow = QualityCategory.Awful;
                    building_press.amount = 0;
                    
                    GenSpawn.Spawn(newCheese, building_press.Position - GenAdj.CardinalDirections[0], building_press.Map);
                    StoragePriority currentPriority = StoreUtility.CurrentStoragePriorityOf(newCheese);
                    IntVec3 c;
                    if (StoreUtility.TryFindBestBetterStoreCellFor(newCheese, this.pawn, this.Map, currentPriority, this.pawn.Faction, out c, true))
                    {
                        this.job.SetTarget(TargetIndex.C, c);
                        this.job.SetTarget(TargetIndex.B, newCheese);
                        this.job.count = newCheese.stackCount; 
                        
                    }
                    else
                    {
                        this.EndJobWith(JobCondition.Incompletable);
                       

                    }
                },
                defaultCompleteMode = ToilCompleteMode.Instant
            };
            yield return Toils_Reserve.Reserve(TargetIndex.B, 1, -1, null);
            yield return Toils_Reserve.Reserve(TargetIndex.C, 1, -1, null);
            yield return Toils_Goto.GotoThing(TargetIndex.B, PathEndMode.ClosestTouch);
            yield return Toils_Haul.StartCarryThing(TargetIndex.B, false, false, false);
            Toil carryToCell = Toils_Haul.CarryHauledThingToCell(TargetIndex.C);
            yield return carryToCell;
            yield return Toils_Haul.PlaceHauledThingInCell(TargetIndex.C, carryToCell, true);






        }
    }
}