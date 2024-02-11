using Sharky;
using Sharky.Helper;
using Sharky.TypeData;
using static SC2APIProtocol.Weapon.Types;

namespace StarCraft2Bot.Builds.Base.Desires
{
    public class ProductionStructureDesire : IDesire
    {
        private UnitCountService unitCountService;

        private BuildingTypeData? buildingTypeData;

        public UnitTypes StructureType { get; private set; }

        public ValueRange Count { get; private set; }
        
        public MacroData Data { get; private set; }
        
        public bool Enforced { get; set; }

        public int MineralCost => GetMineralCost();

        public int VespeneCost => GetVespeneCost();

        public int TimeCost => GetTimeCost();

        public int GetTimeCost()
        {
            var existingCount = unitCountService.BuildingsDoneAndInProgressCount(StructureType);
            var remainingCount = Count - existingCount;

            if (remainingCount <= 0)
                return 0;


            return remainingCount * buildingTypeData?.Time ?? 0;
        }

        public int GetMineralCost()
        {
            var existingCount = unitCountService.BuildingsDoneAndInProgressCount(StructureType);
            var remainingCount = Count - existingCount;

            if (remainingCount <= 0)
                return 0;

            return remainingCount * buildingTypeData?.Minerals ?? 0;
        }

        public int GetVespeneCost()
        {
            var existingCount = unitCountService.BuildingsDoneAndInProgressCount(StructureType);
            var remainingCount = Count - existingCount;

            if (remainingCount <= 0)
                return 0;

            return remainingCount * buildingTypeData?.Gas ?? 0;
        }

        public ProductionStructureDesire(UnitTypes structureType, ValueRange count, MacroData data, UnitCountService unitCountService)
        {
            StructureType = structureType;
            Count = count;
            Data = data;
            this.unitCountService = unitCountService;

            if (new BuildingDataService().BuildingData().TryGetValue(structureType, out var structureInfo))
            {
                buildingTypeData = structureInfo;
            }
        }

        public void Enforce()
        {
            if (Enforced)
                return;

            Data.DesiredProductionCounts[StructureType] = Count;            

            Enforced = true;
        }
    }
}