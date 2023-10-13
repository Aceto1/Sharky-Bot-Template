namespace StarCraft2Bot.Builds.Base.Desires
{
    public class CustomDesire : IDesire
    {
        public CustomDesire(Action customEnforceFunc)
        {
            CustomEnforceFunc = customEnforceFunc;
        }

        public Action CustomEnforceFunc { get; set; }

        public bool Enforced { get; set; }

        public void Enforce()
        {
            if (Enforced)
                return;

            CustomEnforceFunc();

            Enforced = true;
        }
    }
}
