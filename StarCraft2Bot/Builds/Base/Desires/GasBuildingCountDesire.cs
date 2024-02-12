using SC2APIProtocol;
using Sharky;
using Sharky.Helper;
using Sharky.TypeData;

namespace StarCraft2Bot.Builds.Base.Desires
{
    public class GasBuildingCountDesire : IDesire
    {
        public GasBuildingCountDesire(ValueRange count, MacroData data, UnitCountService unitCountService)
        {
            Count = count;
            Data = data;
            this.unitCountService = unitCountService;

            UnitTypes gasBuilding;

            switch (data.Race)
            {
                case Race.Terran:
                    gasBuilding = UnitTypes.TERRAN_REFINERY;
                    break;
                case Race.Zerg:
                    gasBuilding = UnitTypes.ZERG_EXTRACTOR;
                    break;
                case Race.Protoss:
                    gasBuilding = UnitTypes.PROTOSS_ASSIMILATOR;
                    break;
                default:
                    gasBuilding = UnitTypes.TERRAN_REFINERY;
                    break;
            }

            GasBuildingType = gasBuilding;

            if (new BuildingDataService().BuildingData().TryGetValue(gasBuilding, out var structureInfo))
            {
                buildingTypeData = structureInfo;
            }
        }

        private UnitCountService unitCountService;

        private BuildingTypeData? buildingTypeData;

        public UnitTypes GasBuildingType;

        public ValueRange Count { get; private set; }

        public MacroData Data { get; private set; }

        public bool Enforced { get; set; }

        public int MineralCost => GetMineralCost();

        public int VespeneCost => GetVespeneCost();

        public int TimeCost => GetTimeCost();

        public int GetTimeCost()
        {
            var existingCount = unitCountService.BuildingsDoneAndInProgressCount(GasBuildingType);
            var remainingCount = Count - existingCount;

            if (remainingCount <= 0)
                return 0;


            return remainingCount * buildingTypeData?.Time ?? 0;
        }

        public int GetMineralCost()
        {
            var existingCount = unitCountService.BuildingsDoneAndInProgressCount(GasBuildingType);
            var remainingCount = Count - existingCount;

            if (remainingCount <= 0)
                return 0;

            return remainingCount * buildingTypeData?.Minerals ?? 0;
        }

        public int GetVespeneCost()
        {
            var existingCount = unitCountService.BuildingsDoneAndInProgressCount(GasBuildingType);
            var remainingCount = Count - existingCount;

            if (remainingCount <= 0)
                return 0;

            return remainingCount * buildingTypeData?.Gas ?? 0;
        }

        public void Enforce()
        {
            if (Enforced)
                return;

            Data.DesiredGases = Count;
            Enforced = true;
        }
    }
}
