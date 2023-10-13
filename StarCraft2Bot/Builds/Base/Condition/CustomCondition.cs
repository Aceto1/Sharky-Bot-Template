namespace StarCraft2Bot.Builds.Base.Condition
{
    public class CustomCondition : ICondition
    {
        public CustomCondition(Func<bool> func)
        {
            CustomConditionFunc = func;
        }

        public Func<bool> CustomConditionFunc { get; private set; }

        public bool IsFulfilled()
        {
            return CustomConditionFunc();
        }
    }
}
