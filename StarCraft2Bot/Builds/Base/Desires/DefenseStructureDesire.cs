using Sharky;
using Sharky.TypeData;

namespace StarCraft2Bot.Builds.Base.Desires
{
    public class DefenseStructureDesire : IDesire
    {
        private UnitCountService unitCountService;

        private BuildingTypeData? buildingTypeData;

        public UnitTypes StructureType { get; private set; }
        
        public int Count { get; private set; }
        
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

            return existingCount * buildingTypeData?.Minerals ?? 0;
        }

        public int GetVespeneCost()
        {
            var existingCount = unitCountService.BuildingsDoneAndInProgressCount(StructureType);
            var remainingCount = Count - existingCount;

            if (remainingCount <= 0)
                return 0;

            return existingCount * buildingTypeData?.Gas ?? 0;
        }

        public DefenseStructureDesire(UnitTypes structureType, int count, MacroData data, UnitCountService unitCountService)
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

            Data.DesiredDefensiveBuildingsCounts[StructureType] = Count;            

            Enforced = true;
        }
    }
}