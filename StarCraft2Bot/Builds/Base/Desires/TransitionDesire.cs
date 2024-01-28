namespace StarCraft2Bot.Builds.Base.Desires
{
    public class TransitionDesire : IDesire
    {
        public TransitionDesire(Build currentBuild)
        {
            CurrentBuild = currentBuild;
        }

        public TransitionDesire(Build currentBuild, int mineralCost, int vespeneCost, int timeCost)
        {
            CurrentBuild = currentBuild;

            MineralCost = mineralCost;
            VespeneCost = vespeneCost;
            TimeCost = timeCost;
        }

        public bool Enforced { get; set; }
        
        public int MineralCost { get; }
        
        public int VespeneCost { get; }
        
        public int TimeCost { get; }

        public Build CurrentBuild { get; set; }

        public void Enforce()
        {
            if (Enforced)
                return;

            CurrentBuild.DoTransition = true;

            Enforced = true;
        }
    }
}
