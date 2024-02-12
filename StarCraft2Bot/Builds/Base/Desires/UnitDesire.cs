using Sharky;
using Sharky.Helper;
using Sharky.TypeData;

namespace StarCraft2Bot.Builds.Base.Desires
{
    public class UnitDesire : IDesire
    {
        public UnitDesire(UnitTypes unit, ValueRange count, Dictionary<UnitTypes, ValueRange> dataDict, UnitCountService unitCountService)
        {
            this.dataDict = dataDict;
            Unit = unit;
            Count = count;
            this.unitCountService = unitCountService;

            if (new TrainingDataService().TrainingData().TryGetValue(unit, out var unitInfo))
            {
                typeData = unitInfo;
            }
        }

        public void Enforce()
        {
            if (Enforced)
                return;

            if (!dataDict.TryAdd(Unit, Count))
                dataDict[Unit] = Count;

            Enforced = true;
        }

        private readonly Dictionary<UnitTypes, ValueRange> dataDict;

        private UnitCountService unitCountService;

        private TrainingTypeData? typeData;

        public ValueRange Count { get; private set; }

        public UnitTypes Unit { get; private set; }

        public bool Enforced { get; set; }

        public int MineralCost => GetMineralCost();

        public int VespeneCost => GetVespeneCost();

        public int TimeCost => GetTimeCost();

        public int GetTimeCost()
        {
            var existingCount = unitCountService.BuildingsDoneAndInProgressCount(Unit);
            var remainingCount = Count - existingCount;

            if (remainingCount <= 0 || typeData == null)
                return 0;

            var buildingData = new BuildingDataService().BuildingData();

            var buildingTypes = typeData.ProducingUnits.ToList();
            var buildingCount = 0;

            foreach (var buildingType in buildingTypes)
            {
                if (!buildingData.ContainsKey(buildingType))
                    continue;

                buildingCount += unitCountService.Count(buildingType);
            }

            if (buildingCount == 0)
                return remainingCount * typeData.Time;

            return Math.Min((int)Math.Ceiling((double)(remainingCount * typeData.Time / buildingCount)), typeData.Time);
        }

        public int GetMineralCost()
        {
            var existingCount = unitCountService.BuildingsDoneAndInProgressCount(Unit);
            var remainingCount = Count - existingCount;

            if (remainingCount <= 0)
                return 0;

            return remainingCount * typeData?.Minerals ?? 0;
        }

        public int GetVespeneCost()
        {
            var existingCount = unitCountService.BuildingsDoneAndInProgressCount(Unit);
            var remainingCount = Count - existingCount;

            if (remainingCount <= 0)
                return 0;

            return remainingCount * typeData?.Gas ?? 0;
        }
    }
}