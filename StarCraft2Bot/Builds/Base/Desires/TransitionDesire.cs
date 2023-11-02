namespace StarCraft2Bot.Builds.Base.Desires
{
    public class TransitionDesire : IDesire
    {
        public TransitionDesire(Build build)
        {
            Build = build;
        }

        public bool Enforced { get; set; }

        public Build Build { get; set; }

        public void Enforce()
        {
            if (Enforced)
                return;

            Build.DoTransition = true;

            Enforced = true;
        }
    }
}
