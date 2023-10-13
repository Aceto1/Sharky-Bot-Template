using Sharky;

namespace StarCraft2Bot.Builds.Base.Desires
{
    public class SupplyDepotDesire : IDesire
    {
        public SupplyDepotDesire(int count, MacroData data)
        {
            Count = count;
            Data = data;
        }

        public int Count { get; private set; }

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
