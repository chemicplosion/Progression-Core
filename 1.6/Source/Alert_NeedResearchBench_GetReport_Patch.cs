using HarmonyLib;
using RimWorld;

namespace ProgressionCore
{
    [HarmonyPatch(typeof(Alert_NeedResearchBench), nameof(Alert_NeedResearchBench.GetReport))]
    public static class Alert_NeedResearchBench_GetReport_Patch
    {
        private static void Postfix(ref AlertReport __result)
        {
            if (__result.active && Alert_NeedResearchCampfire.GatheringFireDefs() != null)
            {
                __result = false;
            }
        }
    }
}
