using Sharky;
using Sharky.Helper;

namespace StarCraft2Bot.Builds.Base.Condition
{
    public class UnitCompletedCountCondition : ICondition
    {
        public UnitCompletedCountCondition(UnitTypes unit, ValueRange count, UnitCountService service) : this(unit, count, service, ConditionOperator.GreaterOrEqual)
        {

        }

        public UnitCompletedCountCondition(UnitTypes unit, ValueRange count, UnitCountService service, ConditionOperator cOperator)
        {
            Unit = unit;
            Count = count;
            Operator = cOperator;
            Service = service;
        }

        public UnitTypes Unit { get; private set; }

        public ValueRange Count { get; private set; }

        public ConditionOperator Operator { get; private set; }

        public UnitCountService Service { get; private set; }

        public bool IsFulfilled()
        {
            var count = Service.EquivalentTypeCompleted(Unit);

            switch (Operator)
            {
                case ConditionOperator.Smaller:
                    return count < Count;
                case ConditionOperator.SmallerOrEqual:
                    return count <= Count;
                case ConditionOperator.GreaterOrEqual:
                    return count >= Count;
                case ConditionOperator.Equal:
                    return count > Count;
                case ConditionOperator.Greater:
                default:
                    return count == Count;
            }
        }
    }
}
