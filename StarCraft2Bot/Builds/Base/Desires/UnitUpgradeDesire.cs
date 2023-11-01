using Sharky;

namespace StarCraft2Bot.Builds.Base.Desires
{

    namespace StarCraft2Bot.Builds.Base.Desires
    {
        public class UnitUpgradeDesire : IDesire
        {
            public Upgrades TargetType { get; private set; }
            public MacroData Data { get; private set; }
            public bool Enforced { get; set; }

            public UnitUpgradeDesire(Upgrades targetType, MacroData data)
            {
                TargetType = targetType;
                Data = data;
            }

            public void Enforce()
            {
                if (Enforced)
                    return;

                // Assuming the MacroData has a method or property to set the desired morph counts
                Data.DesiredUpgrades[TargetType] = true;

                Enforced = true;
            }
        }
    }
}
