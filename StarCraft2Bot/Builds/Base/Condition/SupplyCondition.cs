using Sharky;
using Sharky.Helper;

namespace StarCraft2Bot.Builds.Base.Condition
{
    public class SupplyCondition : ICondition
    {
        public SupplyCondition(ValueRange supplyCount, MacroData data): this(supplyCount, data, ConditionOperator.GreaterOrEqual)
        {
            
        }

        public SupplyCondition(ValueRange supplyCount, MacroData data, ConditionOperator conditionOperator)
        {
            SupplyCount = supplyCount;
            Operator = conditionOperator;
            Data = data;
        }

        public ValueRange SupplyCount { get; private set; }

        public ConditionOperator Operator { get; private set; }

        public MacroData Data { get; private set; }

        public bool IsFulfilled()
        {
            var supply = Data.FoodUsed;

            switch (Operator)
            {
                case ConditionOperator.Smaller:
                    return supply < SupplyCount;
                case ConditionOperator.SmallerOrEqual:
                    return supply <= SupplyCount;
                case ConditionOperator.GreaterOrEqual:
                    return supply >= SupplyCount;
                case ConditionOperator.Greater:
                    return supply > SupplyCount;
                case ConditionOperator.Equal:
                default:
                    return supply == SupplyCount;
            }
        }
    }
}
