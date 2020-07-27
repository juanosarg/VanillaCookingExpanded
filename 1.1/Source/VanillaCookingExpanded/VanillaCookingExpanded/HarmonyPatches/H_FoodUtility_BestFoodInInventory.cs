using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using RimWorld;
using Verse;

namespace VanillaCookingExpanded.HarmonyPatches
{
    [HarmonyPatch(typeof(FoodUtility), nameof(FoodUtility.BestFoodInInventory))]
    public static class H_FoodUtility_BestFoodInInventory
    {
        private static MethodInfo _isPawnDesperate =
            typeof(H_FoodUtility_BestFoodInInventory).GetMethod(nameof(IsPawnDesperate), AccessTools.all);

        private static MethodInfo _checkCondiment =
            typeof(PawnUtility).GetMethod(nameof(PawnUtility.PawnNotHaveActiveCondimentAsFood), AccessTools.all);

        private static List<CodeInstruction> _loopStartPattern = new List<CodeInstruction>()
        {
            // It matches the pattern "Thing thing = innerContainer[i];"
            new CodeInstruction(OpCodes.Ldloc_0),
            new CodeInstruction(OpCodes.Ldloc_1),
            new CodeInstruction(OpCodes.Callvirt),
            new CodeInstruction(OpCodes.Stloc_2),
        };

        private static List<CodeInstruction> _loopHeadPattern = new List<CodeInstruction>()
        {
            // It matches "i++" in a loop header
            new CodeInstruction(OpCodes.Ldloc_1),
            new CodeInstruction(OpCodes.Ldc_I4_1),
            new CodeInstruction(OpCodes.Add),
            new CodeInstruction(OpCodes.Stloc_1),
        };

        [HarmonyDebug]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> codeInstructions, ILGenerator ilGenerator)
        {
            /***** Before Patch ******/
            //ThingOwner<Thing> innerContainer = holder.inventory.innerContainer;
            //for (int i = 0; i < innerContainer.Count; i++)
            //{
            //	Thing thing = innerContainer[i];
            //	if (thing.def.IsNutritionGivingIngestible && thing.IngestibleNow && eater.WillEat(thing, holder) && (int)thing.def.ingestible.preferability >= (int)minFoodPref && (int)thing.def.ingestible.preferability <= (int)maxFoodPref && (allowDrug || !thing.def.IsDrug) && thing.GetStatValue(StatDefOf.Nutrition) * (float)thing.stackCount >= minStackNutrition)
            //	{
            //		return thing;
            //	}
            //}

            /***** After Patch *****/
            //ThingOwner<Thing> innerContainer = holder.inventory.innerContainer;

            ////**** new code ****////
            //// bool desperate = IsPawnDesperate(eater);
            ////**** new code end ****////

            //for (int i = 0; i < innerContainer.Count; i++)
            //{
            //	Thing thing = innerContainer[i];

                ////**** new code ****////
                //// if (!CheckCondiment(thing, desperate, eater))
                ////    continue;
                ////*** new code end ****////

            //	if (thing.def.IsNutritionGivingIngestible && thing.IngestibleNow && eater.WillEat(thing, holder) && (int)thing.def.ingestible.preferability >= (int)minFoodPref && (int)thing.def.ingestible.preferability <= (int)maxFoodPref && (allowDrug || !thing.def.IsDrug) && thing.GetStatValue(StatDefOf.Nutrition) * (float)thing.stackCount >= minStackNutrition)
            //	{
            //		return thing;
            //	}
            //}

            List<CodeInstruction> code = new List<CodeInstruction>(codeInstructions);
            int patternCount = _loopStartPattern.Count;

            for (int i = 0; i < code.Count; i++)
            {
                if (MatchPattern(i, _loopStartPattern, code))
                {
                    Label loopHeadLabel = ilGenerator.DefineLabel();
                    for (int j = i + 1; j < code.Count; j++)
                    {
                        if (MatchPattern(j, _loopHeadPattern, code))
                        {
                            code[j].labels.Add(loopHeadLabel);
                            break;
                        }
                    }

                    LocalBuilder _desperate = ilGenerator.DeclareLocal(typeof(bool));
                    code.Insert(i, new CodeInstruction(OpCodes.Ldarg_1)); // eater
                    code.Insert(i + 1, new CodeInstruction(OpCodes.Call, _isPawnDesperate));
                    code.Insert(i + 2, new CodeInstruction(OpCodes.Stloc, _desperate.LocalIndex));
                    code.Insert(i + 3 + patternCount, new CodeInstruction(OpCodes.Ldloc_2)); // Thing
                    code.Insert(i + 4 + patternCount, new CodeInstruction(OpCodes.Ldloc, _desperate.LocalIndex)); // Desperate
                    code.Insert(i + 5 + patternCount, new CodeInstruction(OpCodes.Ldarg_1)); // eater
                    code.Insert(i + 6 + patternCount, new CodeInstruction(OpCodes.Call, _checkCondiment));
                    code.Insert(i + 7 + patternCount, new CodeInstruction(OpCodes.Brfalse, loopHeadLabel));

                    break;
                }
            }

            return code;
        }

        private static bool MatchPattern(int index, List<CodeInstruction> pattern, List<CodeInstruction> codeInstructions)
        {
            for (int i = index, j = 0; i < codeInstructions.Count && j < pattern.Count; i++, j++)
            {
                if (codeInstructions[i].opcode == pattern[j].opcode)
                    continue;

                return false;
            }

            return true;
        }

        private static bool IsPawnDesperate(Pawn pawn)
        {
            return pawn.needs.food.CurCategory == HungerCategory.Starving;
        }
    }
}
