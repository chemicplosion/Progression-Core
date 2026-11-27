using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace ProgressionCore
{
    public class Alert_NeedResearchCampfire : Alert
    {
        public Alert_NeedResearchCampfire()
        {
            defaultLabel = "ProgressionCore_NeedResearchCampfire".Translate();
        }

        public override AlertReport GetReport()
        {
            if (Find.CurrentGravship != null)
            {
                return false;
            }

            var fireDefs = GatheringFireDefs();
            return fireDefs != null && !Find.Maps.Any(map =>
                fireDefs.Any(map.listerBuildings.ColonistsHaveBuilding));
        }

        public override TaggedString GetExplanation()
        {
            return "ProgressionCore_NeedResearchCampfireDesc".Translate();
        }

        public override void OnClick()
        {
            Find.MainTabsRoot.SetCurrentTab(MainButtonDefOf.Research);
        }

        internal static List<ThingDef> GatheringFireDefs()
        {
            var faction = Faction.OfPlayerSilentFail;
            if (!ModsConfig.IdeologyActive || faction == null || Find.CurrentMap == null
                || faction.def.techLevel < TechLevel.Animal || faction.def.techLevel > TechLevel.Neolithic)
            {
                return null;
            }

            var project = Find.ResearchManager.GetProject();
            if (project == null || project.techLevel < TechLevel.Animal
                || project.techLevel > TechLevel.Neolithic || !project.CanStartNow)
            {
                return null;
            }

            var outcome = DefDatabase<RitualOutcomeEffectDef>.GetNamedSilentFail("VFET_TribalGathering_Outcome");
            var fires = outcome?.comps?.OfType<RitualOutcomeComp_RitualTargetDefs>().FirstOrDefault()?.defs;
            if (fires.NullOrEmpty() || Find.Maps.Any(map => map.listerBuildings.ColonistsHaveResearchBench())
                || DefDatabase<DesignationCategoryDef>.AllDefsListForReading.Any(category =>
                    category.ResolvedAllowedDesignators.Any(IsVisibleBenchDesignator)))
            {
                return null;
            }

            return fires;
        }

        private static bool IsVisibleBenchDesignator(Designator designator)
        {
            if (designator is Designator_Dropdown dropdown)
            {
                return dropdown.Elements.Any(IsVisibleBenchDesignator);
            }

            return designator is Designator_Build build && build.PlacingDef is ThingDef def
                && def.thingClass != null && typeof(Building_ResearchBench).IsAssignableFrom(def.thingClass)
                && build.Visible;
        }
    }
}
