using SC2APIProtocol;
using Sharky;
using Sharky.Chat;
using Sharky.Managers;
using StarCraft2Bot.Database;
using Action = SC2APIProtocol.Action;
using Game = StarCraft2Bot.Database.Entities.Game;

namespace StarCraft2Bot.Bot;

public class CustomSharkyBot : SharkyBot
{
    private readonly BaseBot bot;

    public CustomSharkyBot(List<IManager> managers, DebugService debugService,
        FrameToTimeConverter frameToTimeConverter, SharkyOptions sharkyOptions, PerformanceData performanceData,
        ChatService chatService, TagService tagService) : base(managers, debugService, frameToTimeConverter,
        sharkyOptions, performanceData, chatService, tagService)
    {
        throw new NotImplementedException("Use the other constructor!");
    }

    public CustomSharkyBot(List<IManager> managers, DebugService debugService,
        FrameToTimeConverter frameToTimeConverter, SharkyOptions sharkyOptions, PerformanceData performanceData,
        ChatService chatService, TagService tagService, BaseBot baseBot) : base(managers, debugService,
        frameToTimeConverter, sharkyOptions, performanceData, chatService, tagService)
    {
        bot = baseBot;
    }

    public static int GameId { get; private set; } = -1;


    public override void OnEnd(ResponseObservation observation, Result result)
    {
        base.OnEnd(observation, result);

        using var ctx = new DatabaseContext();

        var game = ctx.Games.First(m => m.Id == GameId);

        game.Result = result;
        game.GameLength = (int)Math.Round(FrameToTimeConverter.GetTime(bot.Frame).TotalSeconds);

        ctx.SaveChanges();
    }

    public override void OnStart(ResponseGameInfo gameInfo, ResponseData data, ResponsePing pingResponse,
        ResponseObservation observation, uint playerId, string opponentId)
    {
        using var ctx = new DatabaseContext();

        var myRace = Race.NoRace;
        var enemyRace = Race.NoRace;

        foreach (var playerInfo in gameInfo.PlayerInfo)
            if (playerInfo.PlayerId == playerId)
                myRace = playerInfo.RaceActual;
            else
                enemyRace = playerInfo.RaceRequested;

        var game = new Game
        {
            GameStart = DateTime.UtcNow,
            GameLength = -1,
            MyRace = myRace,
            EnemyRace = enemyRace,
            Result = Result.Undecided,
            MapName = gameInfo.MapName
        };

        ctx.Games.Add(game);
        ctx.SaveChanges();
        GameId = game.Id;

        base.OnStart(gameInfo, data, pingResponse, observation, playerId, opponentId);
    }

    public override IEnumerable<Action> OnFrame(ResponseObservation observation)
    {
        bot.Frame++;

        return base.OnFrame(observation);
    }
}