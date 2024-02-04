namespace StarCraft2Bot.Builds.Base.Desires
{
    public class CustomDesire : IDesire
    {
        public CustomDesire(System.Action customEnforceFunc)
        {
            CustomEnforceFunc = customEnforceFunc;
        }

        public CustomDesire(System.Action customEnforceFunc, int mineralCost, int vespeneCost, int timeCost)
        {
            CustomEnforceFunc = customEnforceFunc;

            MineralCost = mineralCost;
            VespeneCost = vespeneCost;
            TimeCost = timeCost;
        }

        public System.Action CustomEnforceFunc { get; set; }

        public bool Enforced { get; set; }

        public int MineralCost { get; }

        public int VespeneCost { get; }

        public int TimeCost { get; }

        public void Enforce()
        {
            if (Enforced)
                return;

            CustomEnforceFunc();

            Enforced = true;
        }
    }
}
