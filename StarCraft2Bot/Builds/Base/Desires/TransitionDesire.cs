namespace StarCraft2Bot.Builds.Base.Desires
{
    public class TransitionDesire : IDesire
    {
        public TransitionDesire(Build currentBuild)
        {
            CurrentBuild = currentBuild;
        }

        public bool Enforced { get; set; }

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
