using Sharky;
using Sharky.Helper;

namespace StarCraft2Bot.Builds.Base.Desires
{
    public class SupplyDepotDesire : IDesire
    {
        public SupplyDepotDesire(ValueRange count, MacroData data)
        {
            Count = count;
            Data = data;
        }

        public ValueRange Count { get; private set; }

        public MacroData Data { get; private set; }

        public bool Enforced { get; set; }

        public void Enforce()
        {
            if (Enforced)
                return;

            Data.DesiredSupplyDepots = Count;

            Enforced = true;
        }
    }
}
