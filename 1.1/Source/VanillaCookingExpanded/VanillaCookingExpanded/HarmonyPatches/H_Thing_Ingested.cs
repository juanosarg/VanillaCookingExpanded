using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using RimWorld;
using Verse;

namespace VanillaCookingExpanded.HarmonyPatches
{
    [HarmonyPatch(typeof(Thing), nameof(Thing.Ingested))]
    public static class H_Thing_Ingested
    {
        private static readonly FieldInfo _tolerances =
            typeof(JoyToleranceSet).GetField($"tolerances", BindingFlags.NonPublic | BindingFlags.Instance);

        private static readonly FieldInfo _bored =
            typeof(JoyToleranceSet).GetField($"bored", BindingFlags.NonPublic | BindingFlags.Instance);

        public static bool Prefix(Pawn ingester, Thing __instance, ref JoyState __state)
        {
            ThingDef ingestThingDef = __instance.def;
            JoyToleranceSet set = ingester?.needs?.joy?.tolerances;
            if (ingestThingDef == null || set == null || !DessertDefs.AllDeserts.Contains(ingestThingDef))
            {
                __state = new JoyState(false, 0, null);
                return true;
            }

            DefMap<JoyKindDef, float> tolerances = (DefMap<JoyKindDef, float>)_tolerances.GetValue(set);
            __state = new JoyState(true, tolerances[JoyKindDefOf.Gluttonous], tolerances);
            return true;
        }

        public static void Postfix(Pawn ingester, ref JoyState __state)
        {
            if (!__state.IsVCEDesert)
                return;

            if (__state.PreviousTolerance.EqualsTo(0))
                __state.PreviousTolerance = 0f;

            if (__state.PreviousTolerance < 0)
                return;

            __state.Tolerances[JoyKindDefOf.Gluttonous] = __state.PreviousTolerance;

            if (__state.PreviousTolerance <= 0.5f)
            {
                DefMap<JoyKindDef, bool> bored = (DefMap<JoyKindDef, bool>)_bored.GetValue(ingester.needs.joy.tolerances);
                bored[JoyKindDefOf.Gluttonous] = false;
            }
        }

        public struct JoyState
        {

            public bool IsVCEDesert;

            public float PreviousTolerance;

            public DefMap<JoyKindDef, float> Tolerances;

            public JoyState(bool isVceDesert, float previousJoy, DefMap<JoyKindDef, float> tolerances)
            {
                IsVCEDesert = isVceDesert;
                PreviousTolerance = previousJoy;
                Tolerances = tolerances;
            }
        }
    }
}
