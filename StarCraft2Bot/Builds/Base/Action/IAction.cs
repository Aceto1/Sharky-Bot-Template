using StarCraft2Bot.Builds.Base.Condition;
using StarCraft2Bot.Builds.Base.Desires;

namespace StarCraft2Bot.Builds.Base.Action
{
    public interface IAction
    {
        public int MineralCost { get; }
        public int TimeCost { get; }
        public int VespeneCost { get; }

        public bool HasStarted();
        public bool HasSpendResources();
        public bool HasCompleted();

        public List<ICondition> GetConditions();
        public List<IDesire> GetDesires();

        public bool AreConditionsFulfilled();
        public void Enforce();
    }
}