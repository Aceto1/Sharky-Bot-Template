using Sharky;
using Sharky.Builds;
using System;

namespace StarCraft2Bot.Builds.Base.Desires
{
    public class ProxyProductionStructureDesire : IDesire
    {
        public UnitTypes StructureType { get; private set; }
        public int Count { get; private set; }
        public MacroData Data { get; private set; }
        public string ProxyName { get; private set; }
        public bool Enforced { get; set; }

        public ProxyProductionStructureDesire(UnitTypes structureType, int count, MacroData data, string proxyName)
        {
            StructureType = structureType;
            Count = count;
            Data = data;
            ProxyName = proxyName;
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