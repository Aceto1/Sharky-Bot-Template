using Sharky;
using StarCraft2Bot.Bot;
using StarCraft2Bot.Builds.Base.Condition;
using StarCraft2Bot.Builds.Base.Desires;
using StarCraft2Bot.Helper;

namespace StarCraft2Bot.Builds.Base.Action.BuildBlocks
{
    public class AutoTechBuildBlock(string name, BaseBot bot) : BuildBlock(name)
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

        public void PrintBuildBlock()
        {
            ActionNode.PrintNodeTree(ActionTree, additionalInfos: node => string.Join(",", GetRequiredTechForActionNode(node)));
        }

        private void InjectMissingTechBuildingNodes()
        {
            
        }

        private List<UnitTypes> GetRequiredTechForActionNode(ActionNode node)
        {
            List<UnitTypes> desiredUnits = node.nodeAction != null ? GetDesiredUnitsOfAction(node.nodeAction) : [];
            return desiredUnits.SelectMany(TerranTechTree.GetRecursiveRequiredTechStructuresForUnit).ToList();
        }

        private IDesire GetDesireToBuildTech(UnitTypes unit)
        {
            return new CustomDesire(() => { });
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
