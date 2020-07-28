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

            List<ThoughtDef> hasThouthgs =
                pawn?.needs?.mood?.thoughts?.memories?.Memories
                    .Where(m => ThoughtDefs.AllThoughts.Contains(m.def))
                    .Select(m => m.def)
                    .ToList();

            if (!hasThouthgs?.Any() ?? true)
                return true;

            CompIngredients compIngredients = t.TryGetComp<CompIngredients>();
            if (compIngredients == null)
                return true;

            bool result = !compIngredients.ingredients
                .Where(def => def != null)
                .Any(i => hasThouthgs.Contains(i.ingestible.specialThoughtAsIngredient));

            return result;
        }
    }
}
