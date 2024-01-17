using Sharky;
using Sharky.Helper;
using Sharky.TypeData;

namespace StarCraft2Bot.Builds.Base.Desires
{
    public class AddonStructureDesire : IDesire
    {
        public UnitTypes AddonType { get; private set; }

        public ValueRange Count { get; private set; }

        public MacroData Data { get; private set; }

        public bool Enforced { get; set; }

        public int MineralCost { get; }

        public int VespeneCost { get; }

        public int TimeCost { get; }

        public AddonStructureDesire(UnitTypes addonType, ValueRange count, MacroData data)
        {
            AddonType = addonType;
            Count = count;
            Data = data;

            if (new AddOnDataService().AddOnData().TryGetValue(addonType, out var addonInfo))
            {
                MineralCost = addonInfo.Minerals * count;
                VespeneCost = addonInfo.Gas * count;
                TimeCost = addonInfo.Time * count;
            }
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