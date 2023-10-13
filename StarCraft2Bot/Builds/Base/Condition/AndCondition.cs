namespace StarCraft2Bot.Builds.Base.Condition
{
    public class AndCondition : ICondition
    {
        public AndCondition(List<ICondition> conditions)
        {
            Conditions = conditions;
        }

        public AndCondition(params ICondition[] conditions)
        {
            Conditions = conditions.ToList();
        }

        public List<ICondition> Conditions { get; private set; }

        public bool IsFulfilled()
        {
            return Conditions.All(m => m.IsFulfilled());
        }
    }
}
