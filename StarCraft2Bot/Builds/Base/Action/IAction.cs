namespace StarCraft2Bot.Builds.Base.Action
{
    public interface IAction
    {
        public int MineralCost { get; }
        public int TimeCost { get; }
        public int VespeneCost { get; }

        public bool HasStarted();
        public bool HasCompleted();

        public bool AreConditionsFulfilled();
        public void Enforce();
    }
}