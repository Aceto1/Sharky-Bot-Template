using Sharky;
using Sharky.Helper;
using Sharky.TypeData;
using System.Threading;

namespace StarCraft2Bot.Builds.Base.Desires
{
    public class ProxyProductionStructureDesire : IDesire
    {
        public UnitTypes StructureType { get; private set; }
        
        public ValueRange Count { get; private set; }
        
        public MacroData Data { get; private set; }
        
        public string ProxyName { get; private set; }
        
        public bool Enforced { get; set; }
        
        public int MineralCost { get; }
        
        public int VespeneCost { get; }
        
        public int TimeCost { get; }

        public ProxyProductionStructureDesire(UnitTypes structureType, ValueRange count, MacroData data, string proxyName)
        {
            StructureType = structureType;
            Count = count;
            Data = data;
            ProxyName = proxyName;

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

            Data.Proxies[ProxyName].DesiredProductionCounts[StructureType] = Count;            

            Enforced = true;
        }
    }
}