using Sharky;
using Sharky.Helper;
using Sharky.TypeData;

namespace StarCraft2Bot.Builds.Base.Desires
{
    public class SupplyDepotDesire : IDesire
    {
        public SupplyDepotDesire(ValueRange count, MacroData data, UnitCountService unitCountService)
        {
            Count = count;
            Data = data;
            this.unitCountService = unitCountService;

            if (new BuildingDataService().BuildingData().TryGetValue(UnitTypes.TERRAN_SUPPLYDEPOT, out var structureInfo))
            {
                buildingTypeData = structureInfo;
            }
        }

        private UnitCountService unitCountService;

        private BuildingTypeData? buildingTypeData;

        public ValueRange Count { get; private set; }

        public MacroData Data { get; private set; }

        public bool Enforced { get; set; }

        public int MineralCost => GetMineralCost();

        public int VespeneCost => GetVespeneCost();

        public int TimeCost => GetTimeCost();

        public int GetTimeCost()
        {
            var existingCount = unitCountService.BuildingsDoneAndInProgressCount(UnitTypes.TERRAN_SUPPLYDEPOT);
            var remainingCount = Count - existingCount;

            if (remainingCount <= 0)
                return 0;


            return remainingCount * buildingTypeData?.Time ?? 0;
        }

        public int GetMineralCost()
        {
            var existingCount = unitCountService.BuildingsDoneAndInProgressCount(UnitTypes.TERRAN_SUPPLYDEPOT);
            var remainingCount = Count - existingCount;

            if (remainingCount <= 0)
                return 0;

            return existingCount * buildingTypeData?.Minerals ?? 0;
        }

        public int GetVespeneCost()
        {
            var existingCount = unitCountService.BuildingsDoneAndInProgressCount(UnitTypes.TERRAN_SUPPLYDEPOT);
            var remainingCount = Count - existingCount;

            if (remainingCount <= 0)
                return 0;

            return existingCount * buildingTypeData?.Gas ?? 0;
        }

        public void Enforce()
        {
            if (Enforced)
                return;

            Data.DesiredSupplyDepots = Count;

            Enforced = true;
        }
    }
}
