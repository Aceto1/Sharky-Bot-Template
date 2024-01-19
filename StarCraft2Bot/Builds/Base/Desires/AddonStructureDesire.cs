using Sharky;
using Sharky.Helper;
using Sharky.TypeData;

namespace StarCraft2Bot.Builds.Base.Desires
{
    public class AddonStructureDesire : IDesire
    {
        private TrainingTypeData? typeData;

        private UnitCountService unitCountService;

        public UnitTypes AddonType { get; private set; }

        public ValueRange Count { get; private set; }

        public MacroData Data { get; private set; }

        public bool Enforced { get; set; }

        public int MineralCost => GetMineralCost();

        public int VespeneCost => GetVespeneCost();

        public int TimeCost => GetTimeCost();

        public int GetTimeCost()
        {
            var existingCount = unitCountService.BuildingsDoneAndInProgressCount(AddonType);
            var remainingCount = Count - existingCount;

            if (remainingCount <= 0)
                return 0;


            return typeData?.Time ?? 0;
        }

        public int GetMineralCost()
        {
            var existingCount = unitCountService.BuildingsDoneAndInProgressCount(AddonType);
            var remainingCount = Count - existingCount;

            if (remainingCount <= 0)
                return 0;

            return existingCount * typeData?.Minerals ?? 0;
        }

        public int GetVespeneCost()
        {
            var existingCount = unitCountService.BuildingsDoneAndInProgressCount(AddonType);
            var remainingCount = Count - existingCount;

            if (remainingCount <= 0)
                return 0;

            return existingCount * typeData?.Gas ?? 0;
        }

        public AddonStructureDesire(UnitTypes addonType, ValueRange count, MacroData data, UnitCountService unitCountService)
        {
            AddonType = addonType;
            Count = count;
            Data = data;
            this.unitCountService = unitCountService;

            if (new AddOnDataService().AddOnData().TryGetValue(addonType, out var addonInfo))
            {
                typeData = addonInfo;
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