using Sharky;
using Sharky.Helper;

namespace StarCraft2Bot.Builds.Base.Condition
{
    public class MineralCondition : ICondition
    {
        public MineralCondition(ValueRange minerals, MacroData data): this(minerals, data, ConditionOperator.GreaterOrEqual)
        {
            
        }

        public MineralCondition(ValueRange minerals, MacroData data, ConditionOperator conditionOperator)
        {
            Minerals = minerals;
            Operator = conditionOperator;
            Data = data;
        }

        public ValueRange Minerals { get; private set; }

        public ConditionOperator Operator { get; private set; }

        public MacroData Data { get; private set; }

        public bool IsFulfilled()
        {
            var minerals = Data.Minerals;

            switch (Operator)
            {
                case ConditionOperator.Smaller:
                    return minerals < Minerals;
                case ConditionOperator.SmallerOrEqual:
                    return minerals <= Minerals;
                case ConditionOperator.GreaterOrEqual:
                    return minerals >= Minerals;
                case ConditionOperator.Greater:
                    return minerals > Minerals;
                case ConditionOperator.Equal:
                default:
                    return minerals == Minerals;
            }
        }
    }
}
