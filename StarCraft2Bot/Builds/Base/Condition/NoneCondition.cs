namespace StarCraft2Bot.Builds.Base.Condition
{
    public class NoneCondition : ICondition
    {
        public bool IsFulfilled()
        {
            return true;
        }
    }
}
