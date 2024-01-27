using Sharky;
using StarCraft2Bot.Bot;
using StarCraft2Bot.Builds.Base.Condition;
using StarCraft2Bot.Builds.Base.Desires;
using StarCraft2Bot.Helper;

namespace StarCraft2Bot.Builds.Base.Action.BuildBlocks
{
    public class AutoTechBuildBlock(BaseBot bot) : BuildBlock()
    {
        protected readonly BaseBot DefaultBot = bot;

        public AutoTechBuildBlock WithConditions(params ICondition[] conditions)
        {
            Conditions.AddRange(conditions);
            return this;
        }

        public AutoTechBuildBlock WithActionNodes(ActionNode.PopulateActionTree populateTree)
        {
            populateTree(ActionTree);
            InjectMissingTechBuildingNodes();
            return this;
        }

        private void DebugLogMissingTech()
        {
            foreach (IAction action in Actions)
            {
                List<UnitTypes> desiredUnits = GetDesiredUnitsOfAction(action);
                if (desiredUnits.Count == 0) continue;

                HashSet<UnitTypes> techRequirements = desiredUnits.SelectMany(TerranTechTree.GetRecursiveRequiredTechStructuresForUnit).ToHashSet();
                Console.WriteLine((string.Join(",", desiredUnits) + " requires " + string.Join(",", techRequirements)).Replace("TERRAN_", ""));
            }
        }

        private void InjectMissingTechBuildingNodes()
        {
            DebugLogMissingTech();
        }

        private List<UnitTypes> GetDesiredUnitsOfAction(IAction action)
        {
            return action.GetDesires().Select(GetUnitFromDesire).OfType<UnitTypes>().ToList();
        }

        private UnitTypes? GetUnitFromDesire(IDesire desire)
        {
            return desire switch
            {
                AddonStructureDesire d => d.AddonType,
                DefenseStructureDesire d => d.StructureType,
                GasBuildingCountDesire d => d.GasBuildingType,
                MorphDesire d => d.TargetType,
                ProductionStructureDesire d => d.StructureType,
                ProxyProductionStructureDesire d => d.StructureType,
                SupplyDepotDesire _ => UnitTypes.TERRAN_SUPPLYDEPOT,
                TechStructureDesire d => d.StructureType,
                UnitDesire d => d.Unit,
                _ => null
            };
        }
    }
}
