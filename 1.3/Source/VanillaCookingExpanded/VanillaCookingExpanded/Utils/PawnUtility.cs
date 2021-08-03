using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace VanillaCookingExpanded
{
    public static class PawnUtility
    {
        public static bool PawnNotHaveActiveCondimentAsFood(Thing t, bool desperate, Pawn pawn)
        {
            if (desperate)
                return true;

            List<ThoughtDef> hasThoughts =
                pawn?.needs?.mood?.thoughts?.memories?.Memories
                    .Where(m => ThoughtDefs.AllThoughts.Contains(m.def))
                    .Select(m => m.def)
                    .ToList();

            if (!hasThoughts?.Any() ?? true)
                return true;

            CompIngredients compIngredients = t.TryGetComp<CompIngredients>();
            if (compIngredients == null)
                return true;

            bool result = false;
            try
            {
                result = !compIngredients.ingredients
                    .Any(def => def != null && def.ingestible!=null && hasThoughts.Contains(def.ingestible.specialThoughtAsIngredient));
            }
            catch (Exception e)
            {
                Log.Warning($"Something wonky happens to your meal {t}.\n{e}");
            }

            return result;
        }

        public static bool IsDesperateForFood(this Pawn pawn)
        {
            return pawn.needs.food.CurCategory >= HungerCategory.Hungry;
        }
    }
}