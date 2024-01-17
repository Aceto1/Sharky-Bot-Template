using Sharky;
using Sharky.TypeData;

namespace StarCraft2Bot.Builds.Base.Desires
{
    public class TechStructureDesire : IDesire
    {
        public UnitTypes StructureType { get; private set; }
        
        public int Count { get; private set; }
        
        public MacroData Data { get; private set; }
        
        public bool Enforced { get; set; }
        
        public int MineralCost { get; }
        
        public int VespeneCost { get; }
        
        public int TimeCost { get; }

        public TechStructureDesire(UnitTypes structureType, int count, MacroData data)
        {
            StructureType = structureType;
            Count = count;
            Data = data;

            if (new BuildingDataService().BuildingData().TryGetValue(structureType, out var structureInfo))
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

            Data.DesiredTechCounts[StructureType] = Count;            

            Enforced = true;
        }
    }
}