using Sharky;
using Sharky.Helper;
using Sharky.TypeData;

namespace StarCraft2Bot.Builds.Base.Desires
{
    public class SupplyDepotDesire : IDesire
    {
        public SupplyDepotDesire(ValueRange count, MacroData data)
        {
            Count = count;
            Data = data;

            if (new BuildingDataService().BuildingData().TryGetValue(UnitTypes.TERRAN_SUPPLYDEPOT, out var structureInfo))
            {
                MineralCost = structureInfo.Minerals * count;
                VespeneCost = structureInfo.Gas * count;
                TimeCost = structureInfo.Time * count;
            }
        }

        public ValueRange Count { get; private set; }

        public MacroData Data { get; private set; }

        public bool Enforced { get; set; }
        
        public int MineralCost { get; }
        
        public int VespeneCost { get; }
        
        public int TimeCost { get; }

        public void Enforce()
        {
            if (Enforced)
                return;

            Data.DesiredSupplyDepots = Count;

            Enforced = true;
        }
    }
}
