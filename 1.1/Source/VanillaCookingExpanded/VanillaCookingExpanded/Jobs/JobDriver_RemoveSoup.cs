using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;
using Verse.AI;
using RimWorld;

namespace VanillaCookingExpanded
{
    public class JobDriver_RemoveSoup : JobDriver
    {
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return this.pawn.Reserve(this.job.targetA, this.job, 1, -1, null);
        }

        public override void Notify_PatherFailed()
        {

            Building_ElectricPot building_pot = (Building_ElectricPot)this.job.GetTarget(TargetIndex.A).Thing;

            this.EndJobWith(JobCondition.ErroredPather);

        }

        [DebuggerHidden]
        protected override IEnumerable<Toil> MakeNewToils()
        {
            //Log.Message("I am inside the job now, with "+pawn.ToString(), false);

            this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
            this.FailOnBurningImmobile(TargetIndex.A);
            yield return Toils_General.DoAtomic(delegate
            {
                this.job.count = 1;
            });
           
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
            yield return Toils_General.Wait(240).FailOnDestroyedNullOrForbidden(TargetIndex.A).FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch).WithProgressBarToilDelay(TargetIndex.A, false, -0.5f);
            yield return new Toil
            {
                initAction = delegate
                {

                    Building_ElectricPot building_pot = (Building_ElectricPot)this.job.GetTarget(TargetIndex.A).Thing;
                    building_pot.SoupReadyAndWaitingForPickup = false;
                    Thing newSoup = ThingMaker.MakeThing(ThingDef.Named(building_pot.soupToTurnInto));
                    newSoup.stackCount = building_pot.amount;
                    newSoup.TryGetComp<CompIngredients>().ingredients = building_pot.ingredients;
                    building_pot.soupToTurnInto = "";
                    building_pot.ingredients = null;
                    building_pot.amount = 0;
                    GenSpawn.Spawn(newSoup, building_pot.Position - GenAdj.CardinalDirections[0], building_pot.Map);
                    StoragePriority currentPriority = StoreUtility.CurrentStoragePriorityOf(newSoup);
                    IntVec3 c;
                    if (StoreUtility.TryFindBestBetterStoreCellFor(newSoup, this.pawn, this.Map, currentPriority, this.pawn.Faction, out c, true))
                    {
                        this.job.SetTarget(TargetIndex.C, c);
                        this.job.SetTarget(TargetIndex.B, newSoup);
                        this.job.count = newSoup.stackCount; ;
                        
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