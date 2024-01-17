using SC2APIProtocol;
using Sharky;
using Sharky.Helper;
using Sharky.TypeData;

namespace StarCraft2Bot.Builds.Base.Desires
{
    public class GasBuildingCountDesire : IDesire
    {
        public GasBuildingCountDesire(ValueRange count, MacroData data)
        {
            Count = count;
            Data = data;

            UnitTypes gasBuilding;

            switch (data.Race)
            {
                case Race.Terran:
                    gasBuilding = UnitTypes.TERRAN_REFINERY;
                    break;
                case Race.Zerg:
                    gasBuilding = UnitTypes.ZERG_EXTRACTOR;
                    break;
                case Race.Protoss:
                    gasBuilding = UnitTypes.PROTOSS_ASSIMILATOR;
                    break;
                default:
                    gasBuilding = UnitTypes.TERRAN_REFINERY;
                    break;
            }

            if (new BuildingDataService().BuildingData().TryGetValue(gasBuilding, out var structureInfo))
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

            Data.DesiredGases = Count;

            Enforced = true;
        }
    }
}
