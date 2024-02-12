using Sharky;
using StarCraft2Bot.Bot;
using StarCraft2Bot.Builds.Base.Condition;
using StarCraft2Bot.Builds.Base.Desires;
using StarCraft2Bot.Helper;

namespace StarCraft2Bot.Builds.Base.Action.BuildBlocks
{
    public class AutoTechBuildBlock(string name, BaseBot bot, bool debugMessages = false) : BuildBlock(name)
    {
        protected readonly BaseBot DefaultBot = bot;
        private readonly bool debugMessages = debugMessages;

        public new int MineralCost => base.MineralCost + getTechRequirementCosts().Minerals;
        public new int VespeneCost => base.VespeneCost + getTechRequirementCosts().Gas;
        public new int TimeCost => base.TimeCost + getTechRequirementCosts().Time;

        public AutoTechBuildBlock WithConditions(params ICondition[] conditions)
        {
            Conditions.AddRange(conditions);
            return this;
        }

        public AutoTechBuildBlock WithActionNodes(ActionNode.PopulateActionTree populateTree)
        {
            populateTree(ActionTree);
            return this;
        }

        public override void Enforce()
        {
            if(!HasStarted()) //inject tech requirements for first enforcement
            {
                if (debugMessages)
                {
                    Console.WriteLine(GetBlockAsTreeString());
                    Console.WriteLine("Injecting Tech Requirements...");
                }
                InjectMissingTechBuildingNodes();
                if (debugMessages)
                {
                    Console.WriteLine("Tech Requirements Injected:");
                    Console.WriteLine(GetBlockAsTreeString());
                }
            }

            base.Enforce();
        }

        private (int Minerals, int Gas, int Time) getTechRequirementCosts()
        {
            Dictionary<ActionNode, HashSet<UnitTypes>> requiredNewTechPerNode = CalculateRequiredTechForActionNodes(ActionTree, GetAvailableTechUnits());
            HashSet<UnitTypes> requiredNewTech = requiredNewTechPerNode.Values.SelectMany(u => u).ToHashSet();
            List<IAction> actionsRequired = requiredNewTech.Select(u => new BuildAction(new NoneCondition(), GetDesireForUnit(u))).ToList<IAction>();

            return (actionsRequired.Sum(a => a.MineralCost), actionsRequired.Sum(a => a.VespeneCost), actionsRequired.Sum(a => a.TimeCost));
        }

        private void InjectMissingTechBuildingNodes()
        {
            Dictionary<ActionNode, HashSet<UnitTypes>> requiredNewTechPerNode = CalculateRequiredTechForActionNodes(ActionTree, GetAvailableTechUnits());

            foreach (KeyValuePair<ActionNode, HashSet<UnitTypes>> requiredNewTechOfNode in requiredNewTechPerNode)
            {
                foreach (UnitTypes techUnit in requiredNewTechOfNode.Value)
                {
                    requiredNewTechOfNode.Key.InjectActionBetweenParent("GEN " + techUnit, new BuildAction(new NoneCondition(), GetDesireForUnit(techUnit)));
                }
            }
        }

        private Dictionary<ActionNode, HashSet<UnitTypes>> CalculateRequiredTechForActionNodes(ActionNode node, HashSet<UnitTypes> availableTech)
        {
            Dictionary<ActionNode, HashSet<UnitTypes>> requiredNewTechPerNode = [];
            HashSet<UnitTypes> newBuildTechByDesire = GetDesiredUnitsOfActionNode(node).Where(TerranTechTree.TechUnits.Contains).ToHashSet();
            requiredNewTechPerNode[node] = GetRequiredTechForActionNode(node).Except(availableTech).ToHashSet();
            availableTech = availableTech.Union(newBuildTechByDesire).Union(requiredNewTechPerNode[node]).ToHashSet();

            foreach (ActionNode childNode in node.childrenNodes)
            {
                Dictionary<ActionNode, HashSet<UnitTypes>> requiredNewTechByChildNode = CalculateRequiredTechForActionNodes(childNode, availableTech);
                requiredNewTechPerNode = requiredNewTechPerNode.Union(requiredNewTechByChildNode).ToDictionary();
            }
            return requiredNewTechPerNode;
        }

        private HashSet<UnitTypes> GetRequiredTechForActionNode(ActionNode node) =>
            GetDesiredUnitsOfActionNode(node).SelectMany(TerranTechTree.GetRecursiveRequiredTechStructuresForUnit).ToHashSet();

        private HashSet<UnitTypes> GetDesiredUnitsOfActionNode(ActionNode node) => 
            node.nodeAction?.GetDesires().Select(GetUnitFromDesire).OfType<UnitTypes>().ToHashSet() ?? [];

        private HashSet<UnitTypes> GetAvailableTechUnits() => 
            TerranTechTree.TechUnits.Where(unit => GetDesiredUnitCount(unit) > 0).ToHashSet();

        private IDesire GetDesireForUnit(UnitTypes unit, int amount = 1)
        {
            MacroData md = DefaultBot.MacroData;
            UnitCountService ucs = DefaultBot.UnitCountService;
            amount = Math.Max(amount, GetDesiredUnitCount(unit));
            
            return unit switch
            {
                UnitTypes.TERRAN_SUPPLYDEPOT => new SupplyDepotDesire(amount, md, ucs),
                UnitTypes.TERRAN_COMMANDCENTER => new ProductionStructureDesire(unit, amount, md, ucs),
                UnitTypes.TERRAN_ENGINEERINGBAY => new ProductionStructureDesire(unit, amount, md, ucs),
                UnitTypes.TERRAN_BARRACKS => new ProductionStructureDesire(unit, amount, md, ucs),
                UnitTypes.TERRAN_BARRACKSTECHLAB => new AddonStructureDesire(unit, amount, md, ucs),
                UnitTypes.TERRAN_GHOSTACADEMY => new ProductionStructureDesire(unit, amount, md, ucs),
                UnitTypes.TERRAN_FACTORY => new ProductionStructureDesire(unit, amount, md, ucs),
                UnitTypes.TERRAN_FACTORYTECHLAB => new AddonStructureDesire(unit, amount, md, ucs),
                UnitTypes.TERRAN_ARMORY => new TechStructureDesire(unit, amount, md, ucs),
                UnitTypes.TERRAN_STARPORT => new ProductionStructureDesire(unit, amount, md, ucs),
                UnitTypes.TERRAN_STARPORTTECHLAB => new AddonStructureDesire(unit, amount, md, ucs),
                UnitTypes.TERRAN_FUSIONCORE => new TechStructureDesire(unit, amount, md, ucs),
                _ => new CustomDesire(() => Console.WriteLine("Unable to find Desire for Tech-Unit " + unit))
            };
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
        private int GetDesiredUnitCount(UnitTypes unit)
        {
            MacroData md = DefaultBot.MacroData;
            return unit switch
            {
                UnitTypes.TERRAN_SUPPLYDEPOT => md.DesiredSupplyDepots,
                UnitTypes.TERRAN_COMMANDCENTER => md.DesiredProductionCounts.GetValueOrDefault(unit),
                UnitTypes.TERRAN_ENGINEERINGBAY => md.DesiredProductionCounts.GetValueOrDefault(unit),
                UnitTypes.TERRAN_BARRACKS => md.DesiredProductionCounts.GetValueOrDefault(unit),
                UnitTypes.TERRAN_BARRACKSTECHLAB => md.DesiredAddOnCounts.GetValueOrDefault(unit),
                UnitTypes.TERRAN_GHOSTACADEMY => md.DesiredProductionCounts.GetValueOrDefault(unit),
                UnitTypes.TERRAN_FACTORY => md.DesiredProductionCounts.GetValueOrDefault(unit),
                UnitTypes.TERRAN_FACTORYTECHLAB => md.DesiredAddOnCounts.GetValueOrDefault(unit),
                UnitTypes.TERRAN_ARMORY => md.DesiredTechCounts.GetValueOrDefault(unit),
                UnitTypes.TERRAN_STARPORT => md.DesiredProductionCounts.GetValueOrDefault(unit),
                UnitTypes.TERRAN_STARPORTTECHLAB => md.DesiredAddOnCounts.GetValueOrDefault(unit),
                UnitTypes.TERRAN_FUSIONCORE => md.DesiredTechCounts.GetValueOrDefault(unit),
                _ => -1
            };
        }

        public override string ToString() => Name + "(M" + MineralCost + "|V" + VespeneCost + "|T" + TimeCost + ")\n" + GetBlockAsTreeString();
        private string GetBlockAsTreeString() => ActionNode.GetNodeTreeString(ActionTree, additionalInfos: node => string.Join(",", CalculateRequiredTechForActionNodes(ActionTree, GetAvailableTechUnits())[node]));
    }
}
