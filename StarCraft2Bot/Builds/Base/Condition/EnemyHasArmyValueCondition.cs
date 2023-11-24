using Sharky;

namespace StarCraft2Bot.Builds.Base.Condition
{
    public class EnemyHasArmyValueCondition : ICondition
    {
        public EnemyHasArmyValueCondition(ActiveUnitData activeUnitData, UnitDataService unitDataService, uint armyValue)
        {
            ActiveUnitData = activeUnitData;
            UnitDataService = unitDataService;
            ArmyValue = armyValue;
        }

        public ActiveUnitData ActiveUnitData { get; set; }

        public UnitDataService UnitDataService { get; set; }

        public uint ArmyValue { get; set; }

        public bool IsFulfilled()
        {
            uint currentValue = 0;

            foreach (var enemyUnitsValue in ActiveUnitData.EnemyUnits.Values)
            {
                currentValue += enemyUnitsValue.UnitTypeData.MineralCost + enemyUnitsValue.UnitTypeData.VespeneCost;
            }

            return currentValue >= ArmyValue;
        }
    }
}
