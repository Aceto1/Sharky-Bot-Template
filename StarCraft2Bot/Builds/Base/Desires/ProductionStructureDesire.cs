using Sharky;
using Sharky.Helper;
using Sharky.TypeData;
using static SC2APIProtocol.Weapon.Types;

namespace StarCraft2Bot.Builds.Base.Desires
{
    public class ProductionStructureDesire : IDesire
    {
        public UnitTypes StructureType { get; private set; }
        public ValueRange Count { get; private set; }
        
        public MacroData Data { get; private set; }
        
        public bool Enforced { get; set; }
        
        public int MineralCost { get; }
        
        public int VespeneCost { get; }
        
        public int TimeCost { get; }

        public ProductionStructureDesire(UnitTypes structureType, ValueRange count, MacroData data)
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

            Data.DesiredProductionCounts[StructureType] = Count;            

            Enforced = true;
        }
    }
}