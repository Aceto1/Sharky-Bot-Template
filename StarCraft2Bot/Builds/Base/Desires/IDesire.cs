namespace StarCraft2Bot.Builds.Base.Desires
{
    public interface IDesire
    {
        void Enforce();

        public bool Enforced { get; set; }

        public int MineralCost { get; }

        public int VespeneCost { get; }

        /// <summary>
        /// Cost in seconds
        /// </summary>
        public int TimeCost { get; }
    }
}
