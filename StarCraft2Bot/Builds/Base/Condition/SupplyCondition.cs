using Sharky;

namespace StarCraft2Bot.Builds.Base.Condition
{
    public class SupplyCondition : ICondition
    {
        public SupplyCondition(int supplyCount, MacroData data): this(supplyCount, data, ConditionOperator.Equal)
        {
            
        }

        public SupplyCondition(int supplyCount, MacroData data, ConditionOperator conditionOperator)
        {
            SupplyCount = supplyCount;
            Operator = conditionOperator;
            Data = data;
        }

        public int SupplyCount { get; private set; }

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
