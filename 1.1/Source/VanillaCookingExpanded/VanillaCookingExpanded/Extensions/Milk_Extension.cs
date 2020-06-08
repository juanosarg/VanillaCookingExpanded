
using Verse;
using System.Collections.Generic;
using System.Linq;
using RimWorld;

namespace VanillaCookingExpanded
{
    public class Milk_Extension : DefModExtension
    {
        public int mustCapacity = 10;
        public float awfulQualityAgeDaysThreshold = 1f;
        public float poorQualityAgeDaysThreshold = 3f;
        public float normalQualityAgeDaysThreshold = 8f;
        public float goodQualityAgeDaysThreshold = 14f;
        public float excellentQualityAgeDaysThreshold = 20f;
        public float masterworkQualityAgeDaysThreshold = 50f;
        public float legendaryQualityAgeDaysThreshold = 120f;

        public string cheeseToTurnInto = "Milk";
        public int amount = 10;



    }
}
