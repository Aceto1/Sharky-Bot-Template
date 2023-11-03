using SC2APIProtocol;

namespace StarCraft2Bot.Builds.Base.Condition
{
    public class TimeCondition : ICondition
    {
        public ConditionOperator Operator { get; set; }
        public double FramesPerSecond { get; set; }
        public int TargetFrame { get; private set; }
        private Observation? Observation { get; set; }

        public TimeCondition(double seconds, ConditionOperator conditionOperator = ConditionOperator.GreaterOrEqual, double framesPerSecond = 22.4)
        {
            FramesPerSecond = framesPerSecond;
            Operator = conditionOperator;
            SetTargetFrame(seconds);
        }

        public void SetTargetFrame(double seconds)
        {
            TargetFrame = (int)(seconds * FramesPerSecond);
        }

        public void InsertObservation(Observation observation)
        {
            Observation = observation;
        }

        public bool IsFulfilled(Observation observation)
        {
            var currentFrame = (int)observation.GameLoop;
            switch (Operator)
            {
                case ConditionOperator.Smaller:
                    return currentFrame < TargetFrame;
                case ConditionOperator.SmallerOrEqual:
                    return currentFrame <= TargetFrame;
                case ConditionOperator.GreaterOrEqual:
                    return currentFrame >= TargetFrame;
                case ConditionOperator.Greater:
                    return currentFrame > TargetFrame;
                case ConditionOperator.Equal:
                default:
                    return currentFrame == TargetFrame;
            }
        }

        public bool IsFulfilled()
        {
            if (Observation == null)
            {
                throw new InvalidOperationException("Observation has not been set.");
            }

            return IsFulfilled(Observation);
        }
    }
}
