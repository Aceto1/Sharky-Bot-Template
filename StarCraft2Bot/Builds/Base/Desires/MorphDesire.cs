using Sharky;
using Sharky.Helper;
using Sharky.TypeData;

namespace StarCraft2Bot.Builds.Base.Desires
{
    public class MorphDesire : IDesire
    {
        private UnitCountService unitCountService;

        private TrainingTypeData? typeData;

        public UnitTypes TargetType { get; private set; }

        public ValueRange Count { get; private set; }
        
        public MacroData Data { get; private set; }
        
        public bool Enforced { get; set; }

        public int MineralCost => GetMineralCost();

        public int VespeneCost => GetVespeneCost();

        public int TimeCost => GetTimeCost();

        public int GetTimeCost()
        {
            var existingCount = unitCountService.BuildingsDoneAndInProgressCount(TargetType);
            var remainingCount = Count - existingCount;

            if (remainingCount <= 0)
                return 0;


            return remainingCount * typeData?.Time ?? 0;
        }

        public int GetMineralCost()
        {
            var existingCount = unitCountService.BuildingsDoneAndInProgressCount(TargetType);
            var remainingCount = Count - existingCount;

            if (remainingCount <= 0)
                return 0;

            return existingCount * typeData?.Minerals ?? 0;
        }

        public int GetVespeneCost()
        {
            var existingCount = unitCountService.BuildingsDoneAndInProgressCount(TargetType);
            var remainingCount = Count - existingCount;

            if (remainingCount <= 0)
                return 0;

            return existingCount * typeData?.Gas ?? 0;
        }

        public MorphDesire(UnitTypes targetType, ValueRange count, MacroData data, UnitCountService unitCountService)
        {
            TargetType = targetType;
            Count = count;
            Data = data;
            this.unitCountService = unitCountService;

            if (new MorphDataService().MorphData().TryGetValue(targetType, out var morphInfo))
            {
                typeData = morphInfo;
            }
        }

        public void Enforce()
        {
            if (Enforced)
                return;

            // Assuming the MacroData has a method or property to set the desired morph counts
            Data.DesiredMorphCounts[TargetType] = Count;

            Enforced = true;
        }
    }
}