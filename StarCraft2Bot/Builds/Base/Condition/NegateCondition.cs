namespace StarCraft2Bot.Builds.Base.Condition
{
    public class NegateCondition : ICondition
    {
        public NegateCondition(ICondition condition)
        {
            Condition = condition;
        }

        public ICondition Condition { get; private set; }

        public bool IsFulfilled()
        {
            return !Condition.IsFulfilled();
        }
    }
}
