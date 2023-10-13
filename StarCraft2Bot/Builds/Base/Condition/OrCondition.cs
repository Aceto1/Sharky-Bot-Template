namespace StarCraft2Bot.Builds.Base.Condition
{
    public class OrCondition : ICondition
    {
        public OrCondition(List<ICondition> conditions)
        {
            Conditions = conditions;
        }

        public List<ICondition> Conditions { get; private set; }

        public bool IsFulfilled()
        {
            return Conditions.Any(m => m.IsFulfilled());
        }
    }
}
