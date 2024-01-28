using System.Security.Cryptography;
using StarCraft2Bot.Builds.Base.Condition;
using StarCraft2Bot.Builds.Base.Desires;

namespace StarCraft2Bot.Builds.Base
{
    public class BuildAction
    {
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

        public bool AreConditionsFulfilled()
        {
            return Conditions.All(m => m.IsFulfilled());
        }

        public void EnforceDesires()
        {
            Desires.ForEach(m => m.Enforce());
        }

        public int MineralCost => Desires.Sum(m => m.MineralCost);

        public int VespeneCost => Desires.Sum(m => m.VespeneCost);

        public int TimeCost => Desires.Sum(m => m.TimeCost);

        public List<ICondition> Conditions { get; set; }

        public List<IDesire> Desires { get; set; }
    }
}
