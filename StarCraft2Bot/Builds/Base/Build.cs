using SC2APIProtocol;
using Sharky.Builds.Terran;
using Sharky.DefaultBot;

namespace StarCraft2Bot.Builds.Base
{
    public class Build : TerranSharkyBuild
    {
        private const int secondsPerMeasurement = 1;

        private readonly DefaultSharkyBot defaultSharkyBot;

        private int frame = 0;

        public Build(DefaultSharkyBot defaultSharkyBot) : base(defaultSharkyBot)
        {
            this.defaultSharkyBot = defaultSharkyBot;
        }

        public override void OnFrame(ResponseObservation observation)
        {
            base.OnFrame(observation);

            frame++;

            if (frame % (defaultSharkyBot.SharkyOptions.FramesPerSecond * secondsPerMeasurement) == 0)
            {
                Measure();
            }
        }

        private void Measure()
        {
            //TODO: Measure current state
        }
    }
}
