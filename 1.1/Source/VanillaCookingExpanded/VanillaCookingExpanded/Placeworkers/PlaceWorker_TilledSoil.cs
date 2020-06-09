using RimWorld;
using Verse;

namespace VanillaCookingExpanded
{
    public class PlaceWorker_TilledSoil : PlaceWorker
    {
        public override AcceptanceReport AllowsPlacing(BuildableDef checkingDef, IntVec3 loc, Rot4 rot, Map map, Thing thingToIgnore = null, Thing thing = null)
        {
            bool flag = false;
            foreach (TillableTerrainDef element in DefDatabase<TillableTerrainDef>.AllDefs)
            {
                foreach (string terrain in element.terrains)
                {
                    if (map.terrainGrid.TerrainAt(loc).defName== terrain)
                        
                    {
                       flag = true;
                    }
                }
            }

               
            return flag;
        }
    }
}