using StarCraft2Bot.Builds.Base.Action.BuildBlocks;
using StarCraft2Bot.Builds.Base.Condition;
using StarCraft2Bot.Builds.Base.Desires;

namespace StarCraft2Bot.Builds.Base.Action
{
    public abstract class BuildBlock : IAction
    {
        public int MineralCost => CalculateMineralCost();
        public int VespeneCost => CalculateVespeneCost();
        public int TimeCost => CalculateTimeCost();

        protected List<ICondition> Conditions { get; set; }
        protected List<IAction> SerialBuildActions { get; set;  }
        protected List<IAction> ParallelBuildActions { get; set; }

        protected BuildBlock(List<ICondition> conditions, List<IAction> serialBuildActions, List<IAction> parallelBuildActions)
        {
            Conditions = conditions;
            SerialBuildActions = serialBuildActions;
            ParallelBuildActions = parallelBuildActions;
        }

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
            //TODO Find more accurate way to predict time? Abhängig vom Income(Zeit bis Gas und Mineral gesammelt)?
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
            return MineralCost == 0 && VespeneCost == 0 && TimeCost == 0;
        }

        public List<ICondition> GetConditions()
        {
            return Conditions;
        }

        public List<IDesire> GetDesires()
        {
            return SerialBuildActions.SelectMany(action => action.GetDesires()).Concat(ParallelBuildActions.SelectMany(action => action.GetDesires())).ToList();
        }

        public bool AreConditionsFulfilled()
        {
            return Conditions.All(m => m.IsFulfilled());
        } 

        public void Enforce()
        {
            EnforceParallelActions();
            EnforceSerialActions();
        }

        protected void EnforceParallelActions()
        {
            foreach (IAction action in ParallelBuildActions)
            {
                if (action.AreConditionsFulfilled())
                {
                    action.Enforce();
                }
            }
            ParallelBuildActions = ParallelBuildActions.Where(a => !a.HasCompleted()).ToList();
        }

        protected void EnforceSerialActions()
        {
            if (SerialBuildActions.Count == 0) return;

            IAction nextAction = SerialBuildActions[0];
            if (!nextAction.AreConditionsFulfilled()) return;

            nextAction.Enforce();
            if (nextAction.HasCompleted())
            {
                SerialBuildActions.Remove(nextAction);
            }
        }
    }
}
