using StarCraft2Bot.Builds.Base.Condition;
using StarCraft2Bot.Builds.Base.Desires;

namespace StarCraft2Bot.Builds.Base.Action
{
    public class BuildBlock(string Name) : IAction
    {
        public readonly string Name = Name;

        public int MineralCost => Actions.Sum(a => a.MineralCost);
        public int VespeneCost => Actions.Sum(a => a.VespeneCost);
        public int TimeCost => CalculateTimeCost();

        protected List<ICondition> Conditions { get; set; } = [];
        protected ActionNode ActionTree { get; } = ActionNode.GetRootNode(Name);
        protected List<IAction> Actions => ActionTree.GetRecursiveChildActions();

        public List<ICondition> GetConditions() => Conditions;
        public List<IDesire> GetDesires() => Actions.SelectMany(action => action.GetDesires()).ToList();

        public bool HasStarted() => Actions.Any(a => a.HasStarted());
        public bool HasCompleted() => MineralCost == 0 && VespeneCost == 0 && TimeCost == 0;
        public bool AreConditionsFulfilled() => Conditions.All(m => m.IsFulfilled());

        protected int CalculateTimeCost()
        {
            //TODO Find more accurate way to predict time? Abhängig vom Income(Zeit bis Gas und Mineral gesammelt)?
            var serialTimeCost = Actions.Sum(a => a.TimeCost);
            return serialTimeCost;
        }

        public void Enforce()
        {
            ActionTree.EnforceChildrenNodes();
        }
    }
}
