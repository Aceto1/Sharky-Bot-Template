using Sharky;

namespace StarCraft2Bot.Builds.Base.Condition
{
    public class EnemyHasUnitTypeCondition : ICondition
    {
        public EnemyHasUnitTypeCondition(UnitTypes unitType, UnitCountService unitCountService)
        {
            UnitType = unitType;
            UnitCountService = unitCountService;
        }

        public UnitTypes UnitType { get; set; }

        public UnitCountService UnitCountService { get; set; }

        public bool IsFulfilled()
        {
            return UnitCountService.EquivalentEnemyTypeCount(UnitType) > 0;
        }
    }
}
