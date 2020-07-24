using System;
using System.Collections;
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
    [HarmonyPatch]
    public static class H_FoodUtility_TryFindBestFoodSourceFor
    {
        private const string _validatorName = "foodValidator";

        private static readonly MethodInfo _checkCondiment =
            typeof(H_FoodUtility_TryFindBestFoodSourceFor).GetMethod(nameof(PawnNotHaveActiveCondimentAsFood), AccessTools.all);

        private static FieldInfo _eater;

        private static FieldInfo _desperate;

        [HarmonyTargetMethods]
        public static IEnumerable<MethodBase> TargetMethods()
        {
            Type targetClass = typeof(FoodUtility)
                .GetNestedTypes(AccessTools.all)
                .FirstOrDefault(type => type.GetFields(AccessTools.all).Any(field => field.Name == _validatorName));

            if (targetClass == null)
                return Enumerable.Empty<MethodBase>();

            Log.Message("Found target method");
            _eater = targetClass.GetField("eater", AccessTools.all);
            _desperate = targetClass.GetField("desperate", AccessTools.all);
            Log.Message($"_eater type: {_eater}");
            Log.Message($"_desperate type: {_desperate}");

            return targetClass.GetMethods(AccessTools.all).Where(m => m.HasMethodBody());
        }

        [HarmonyDebug]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> codeInstructions, ILGenerator ilGenerator)
        {
            Log.Message($"Patch once");
            List<CodeInstruction> code = new List<CodeInstruction>(codeInstructions);
            if (code.Count < 2)
                return codeInstructions;

            Log.Message($"1: {code[0].opcode}, 2: {code[1].opcode} operand: {code[1].operand}");
            if (code[0].opcode != OpCodes.Ldarg_1
                || code[1].opcode != OpCodes.Isinst
                || !code[1].OperandIs(typeof(Building_NutrientPasteDispenser)))
            {
                return codeInstructions;
            }

            /******* Last two lines of IL in our target method *******/
            // return true;
            //IL_0287: ldc.i4.1
            //IL_0288: ret

            Log.Message($"Patch twice");

            List<Label> labels = code[code.Count - 2].labels;
            code.RemoveAt(code.Count - 2);
            code.Insert(code.Count - 1, new CodeInstruction(OpCodes.Ldarg_1));
            code[code.Count -2].labels.AddRange(labels);
            code.Insert(code.Count - 1, new CodeInstruction(OpCodes.Ldarg_0));
            code.Insert(code.Count - 1, new CodeInstruction(OpCodes.Ldfld, _desperate));
            code.Insert(code.Count - 1, new CodeInstruction(OpCodes.Ldarg_0));
            code.Insert(code.Count - 1, new CodeInstruction(OpCodes.Ldfld, _eater));
            code.Insert(code.Count - 1, new CodeInstruction(OpCodes.Call, _checkCondiment));

            return code;
        }


        private static bool PawnNotHaveActiveCondimentAsFood(Thing t, bool desperate, Pawn pawn)
        {
            Log.Message($"Patched");
            if (desperate)
                return true;

            List<ThoughtDef> hasThouthgs =
                pawn?.needs?.mood?.thoughts?.memories?.Memories
                    .Where(m => ThoughtDefs.AllThoughts.Contains(m.def))
                    .Select(m => m.def)
                    .ToList();

            if (!hasThouthgs?.Any() ?? true)
                return true;

            Log.Message($"Found thoughts: {string.Join(" - ", hasThouthgs)}");
            CompIngredients compIngredients = t.TryGetComp<CompIngredients>();
            if (compIngredients == null)
                return true;

            bool result = !compIngredients.ingredients.Any(
                i => hasThouthgs.Contains(i.ingestible.specialThoughtAsIngredient));
            Log.Message($"Ingredient thoughts: {string.Join(" - ", compIngredients.ingredients.Select(i => i.ingestible.specialThoughtAsIngredient))}, result: {result}");

            return result;
        }
    }
}
