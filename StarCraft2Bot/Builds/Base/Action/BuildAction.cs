using System.Security.Cryptography;
using StarCraft2Bot.Builds.Base.Condition;
using StarCraft2Bot.Builds.Base.Desires;

namespace StarCraft2Bot.Builds.Base.Action
{
    public class BuildAction : IAction
    {
        public int MineralCost => Desires.Sum(m => m.MineralCost);

        public int VespeneCost => Desires.Sum(m => m.VespeneCost);

        public int TimeCost => Desires.Sum(m => m.TimeCost);

        protected List<ICondition> Conditions { get; set; }

        protected List<IDesire> Desires { get; set; }
        public BuildAction(List<ICondition> conditions, List<IDesire> desires)
        {
            Conditions = conditions;
            Desires = desires;
        }

        public BuildAction(ICondition condition, List<IDesire> desires)
        {
            Conditions = new List<ICondition> { condition };
            Desires = desires;
        }

        public BuildAction(ICondition condition, params IDesire[] desires)
        {
            Conditions = new List<ICondition> { condition };
            Desires = desires.ToList();
        }

        public BuildAction(List<ICondition> conditions, IDesire desire)
        {
            Conditions = conditions;
            Desires = new List<IDesire> { desire };
        }

        public BuildAction(ICondition condition, IDesire desire)
        {
            Conditions = new List<ICondition> { condition };
            Desires = new List<IDesire> { desire };
        }

        public bool HasStarted()
        {
            return Desires.Any(d => d.Enforced);
        }

        public bool HasCompleted ()
        {
            return MineralCost == 0 && VespeneCost == 0 && TimeCost == 0
                && Desires.All(d => d.Enforced);
        }

        public List<ICondition> GetConditions()
        {
            return Conditions;
        }

        public List<IDesire> GetDesires()
        {
            return Desires;
        }

        public bool AreConditionsFulfilled()
        {
            return Conditions.All(m => m.IsFulfilled());
        }

        public void Enforce()
        {
            Desires.ForEach(m => m.Enforce());
        }
    }
}
