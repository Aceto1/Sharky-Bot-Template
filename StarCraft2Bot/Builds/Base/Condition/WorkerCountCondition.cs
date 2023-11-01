using Sharky;
using Sharky.Helper;

namespace StarCraft2Bot.Builds.Base.Condition
{
    public class WorkerCountCondition : ICondition
    {
        public WorkerCountCondition(ValueRange count, UnitCountService service) : this(count, service, ConditionOperator.GreaterOrEqual)
        {

        }

        public WorkerCountCondition(ValueRange count, UnitCountService service, ConditionOperator cOperator)
        {
            WorkerCount = count;
            Operator = cOperator;
            Service = service;
        }

        public ValueRange WorkerCount { get; private set; }

        public ConditionOperator Operator { get; private set; }

        public UnitCountService Service { get; private set; }

        private int GetWorkerCount()
        {
            var count = Service.EquivalentTypeCount(UnitTypes.TERRAN_SCV);

            if (count != 0)
                return count;

            count = Service.EquivalentTypeCount(UnitTypes.PROTOSS_PROBE);

            if (count != 0)
                return count;

            count = Service.EquivalentTypeCount(UnitTypes.ZERG_DRONE);

            return count;
        }

        public bool IsFulfilled()
        {
            var count = GetWorkerCount();

            switch (Operator)
            {
                case ConditionOperator.Smaller:
                    return count < WorkerCount;
                case ConditionOperator.SmallerOrEqual:
                    return count <= WorkerCount;
                case ConditionOperator.GreaterOrEqual:
                    return count >= WorkerCount;
                case ConditionOperator.Greater:
                    return count > WorkerCount;
                case ConditionOperator.Equal:
                default:
                    return count == WorkerCount;
            }
        }
    }
}
