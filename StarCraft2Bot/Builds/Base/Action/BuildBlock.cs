using StarCraft2Bot.Builds.Base.Condition;

namespace StarCraft2Bot.Builds.Base.Action
{
    public abstract class BuildBlock : IAction
    {
        public int MineralCost => CalculateMineralCost();
        public int VespeneCost => CalculateVespeneCost();
        public int TimeCost => CalculateTimeCost();

        protected List<ICondition> Conditions { get; }
        protected List<IAction> SerialBuildActions { get; }
        protected List<IAction> ParallelBuildActions { get; }

        protected BuildBlock(List<ICondition> conditions, List<IAction> serialBuildActions, List<IAction> parallelBuildActions)
        {
            Conditions = conditions;
            SerialBuildActions = serialBuildActions;
            ParallelBuildActions = parallelBuildActions;
        }

        protected BuildBlock() : this([], [], []) { }

        protected int CalculateMineralCost()
        {
            return SerialBuildActions.Sum(a => a.MineralCost) + ParallelBuildActions.Sum(a => a.MineralCost);
        }

        protected int CalculateVespeneCost()
        {
            return SerialBuildActions.Sum(a => a.VespeneCost) + ParallelBuildActions.Sum(a => a.VespeneCost);
        }

        protected int CalculateTimeCost()
        {
            //TODO Better accumulate Time Cost
            var serialTimeCost = SerialBuildActions.Sum(a => a.TimeCost);
            var parallelTimeCost = ParallelBuildActions.Count > 0 ? ParallelBuildActions.Max(a => a.TimeCost) : 0;
            return int.Max(serialTimeCost, parallelTimeCost);
        }

        public bool HasStarted()
        {
            return SerialBuildActions.Any(a => a.HasStarted()) || ParallelBuildActions.Any(a=>a.HasStarted());
        }

        public bool HasCompleted()
        {
            return MineralCost == 0 && VespeneCost == 0 && TimeCost == 0
                && SerialBuildActions.All(a => a.HasCompleted()) && ParallelBuildActions.All(a => a.HasCompleted());
        }

        public bool AreConditionsFulfilled()
        {
            return Conditions.All(m => m.IsFulfilled());
        } 

        public void Enforce()
        {
            foreach (BuildAction buildAction in ParallelBuildActions)
            {
                if (buildAction.AreConditionsFulfilled())
                {
                    buildAction.Enforce();
                }
            }
            foreach (BuildAction buildAction in SerialBuildActions)
            {
                if (buildAction.AreConditionsFulfilled())
                { 
                    buildAction.Enforce();
                } else
                {
                    break;
                }
            }
        }
    }
}
