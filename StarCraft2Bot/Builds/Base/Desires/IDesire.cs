namespace StarCraft2Bot.Builds.Base.Desires
{
    public interface IDesire
    {
        void Enforce();

        public bool Enforced { get; set; }

        //public List<BuildAction> AdditionalActions { get; set; }
    }
}
