using Sharky;
using Sharky.Helper;

namespace StarCraft2Bot.Builds.Base.Condition
{
    public class GasCondition : ICondition
    {
        public GasCondition(ValueRange gas, MacroData data): this(gas, data, ConditionOperator.Equal)
        {
            
        }

        public GasCondition(ValueRange gas, MacroData data, ConditionOperator conditionOperator)
        {
            Gas = gas;
            Operator = conditionOperator;
            Data = data;
        }

        public ValueRange Gas { get; private set; }

        public ConditionOperator Operator { get; private set; }

        public MacroData Data { get; private set; }

        public bool IsFulfilled()
        {
            var gas = Data.VespeneGas;

            switch (Operator)
            {
                case ConditionOperator.Smaller:
                    return gas < Gas;
                case ConditionOperator.SmallerOrEqual:
                    return gas <= Gas;
                case ConditionOperator.GreaterOrEqual:
                    return gas >= Gas;
                case ConditionOperator.Greater:
                    return gas > Gas;
                case ConditionOperator.Equal:
                default:
                    return gas == Gas;
            }
        }
    }
}
