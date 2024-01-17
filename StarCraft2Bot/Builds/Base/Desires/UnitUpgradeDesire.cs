using Sharky;
using Sharky.TypeData;

namespace StarCraft2Bot.Builds.Base.Desires
{
    public class UnitUpgradeDesire : IDesire
    {
        public Upgrades TargetType { get; private set; }
        
        public MacroData Data { get; private set; }
        
        public bool Enforced { get; set; }
        
        public int MineralCost { get; }
        
        public int VespeneCost { get; }
        
        public int TimeCost { get; }

        public UnitUpgradeDesire(Upgrades targetType, MacroData data)
        {
            TargetType = targetType;
            Data = data;

            if (new UpgradeDataService().UpgradeData().TryGetValue(targetType, out var structureInfo))
            {
                MineralCost = structureInfo.Minerals;
                VespeneCost = structureInfo.Gas;
                TimeCost = structureInfo.Time;
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
