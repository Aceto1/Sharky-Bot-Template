using Sharky;
using Sharky.Helper;

namespace StarCraft2Bot.Builds.Base.Desires
{
    public class AddonStructureDesire : IDesire
    {
        public UnitTypes AddonType { get; private set; }
        public ValueRange Count { get; private set; }
        public MacroData Data { get; private set; }
        public bool Enforced { get; set; }

        public AddonStructureDesire(UnitTypes addonType, ValueRange count, MacroData data)
        {
            AddonType = addonType;
            Count = count;
            Data = data;
        }

        public void Enforce()
        {
            if (Enforced)
                return;

            Data.DesiredAddOnCounts[AddonType] = Count;

            Enforced = true;
        }
    }
}