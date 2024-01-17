namespace StarCraft2Bot.Builds.Base.Desires
{
    public class CustomDesire : IDesire
    {
        public CustomDesire(Action customEnforceFunc)
        {
            CustomEnforceFunc = customEnforceFunc;
        }

        public CustomDesire(Action customEnforceFunc, int mineralCost, int vespeneCost, int timeCost)
        {
            CustomEnforceFunc = customEnforceFunc;

            MineralCost = mineralCost;
            VespeneCost = vespeneCost;
            TimeCost = timeCost;
        }

        public Action CustomEnforceFunc { get; set; }

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
