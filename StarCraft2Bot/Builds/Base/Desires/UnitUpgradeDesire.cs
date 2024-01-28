using Sharky;
using Sharky.TypeData;

namespace StarCraft2Bot.Builds.Base.Desires
{
    public class UnitUpgradeDesire : IDesire
    {
        private TrainingTypeData? upgradeTypeData;

        private SharkyUnitData unitData;

        public Upgrades TargetType { get; private set; }
        
        public MacroData Data { get; private set; }
        
        public bool Enforced { get; set; }

        public int MineralCost => GetMineralCost();

        public int VespeneCost => GetVespeneCost();

        public int TimeCost => GetTimeCost();

        public int GetTimeCost()
        {
            if (unitData.ResearchedUpgrades.Contains((uint)TargetType))
                return 0;

            return upgradeTypeData?.Time ?? 0;
        }

        public int GetMineralCost()
        {
            if (unitData.ResearchedUpgrades.Contains((uint)TargetType))
                return 0;

            return upgradeTypeData?.Minerals ?? 0;
        }

        public int GetVespeneCost()
        {
            if(unitData.ResearchedUpgrades.Contains((uint)TargetType))
                return 0;

            return upgradeTypeData?.Gas ?? 0;
        }

        public UnitUpgradeDesire(Upgrades targetType, MacroData data, SharkyUnitData unitData)
        {
            TargetType = targetType;
            Data = data;
            this.unitData = unitData;

            if (new UpgradeDataService().UpgradeData().TryGetValue(targetType, out var upgradeInfo))
            {
                upgradeTypeData = upgradeInfo;
            }
        }

        public void Enforce()
        {
            if (Enforced)
                return;

            Data.DesiredUpgrades[TargetType] = true;

            Enforced = true;
        }
    }
}
