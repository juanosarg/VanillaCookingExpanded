using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Verse;

namespace VanillaCookingExpanded.Utils
{
    [StaticConstructorOnStartup]
    public static class HarmonyUtility
    {
        private const string _harmonyId = "VanillaCookingExpanded";

        public static Harmony Instance = new Harmony(_harmonyId);

        static HarmonyUtility()
        {
            Instance.PatchAll();
        }
    }
}
