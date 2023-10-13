using Sharky;
using Sharky.Helper;

namespace StarCraft2Bot.Builds.Base.Desires
{
    public class UnitDesire : IDesire
    {
        public UnitDesire(UnitTypes unit, ValueRange unitCount, Dictionary<UnitTypes, ValueRange> dataDict)
        {
            this.dataDict = dataDict;
            Unit = unit;
            UnitCount = unitCount;
        }

        public void Enforce()
        {
            if (Enforced)
                return;

            if (!dataDict.TryAdd(Unit, UnitCount))
                dataDict[Unit] = UnitCount;

            Enforced = true;
        }

        private readonly Dictionary<UnitTypes, ValueRange> dataDict;

        public ValueRange UnitCount { get; private set; }

        public UnitTypes Unit { get; private set; }

        public bool Enforced { get; set; }
    }
}