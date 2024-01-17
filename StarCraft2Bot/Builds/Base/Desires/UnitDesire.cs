using Sharky;
using Sharky.Helper;
using Sharky.TypeData;

namespace StarCraft2Bot.Builds.Base.Desires
{
    public class UnitDesire : IDesire
    {
        public UnitDesire(UnitTypes unit, ValueRange count, Dictionary<UnitTypes, ValueRange> dataDict)
        {
            this.dataDict = dataDict;
            Unit = unit;
            Count = count;

            if (new TrainingDataService().TrainingData().TryGetValue(unit, out var structureInfo))
            {
                MineralCost = structureInfo.Minerals * count;
                VespeneCost = structureInfo.Gas * count;
                TimeCost = structureInfo.Time * count;
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

        public ValueRange Count { get; private set; }

        public UnitTypes Unit { get; private set; }

        public bool Enforced { get; set; }
        public int MineralCost { get; }
        
        public int VespeneCost { get; }
        
        public int TimeCost { get; }
    }
}