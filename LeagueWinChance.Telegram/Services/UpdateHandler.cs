using ConsoleTables;
using LeagueWinChance.Core;
using MingweiSamuel.Camille.Enums;
using MingweiSamuel.Camille.LolStatusV3;
using System.Security.Principal;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.ReplyMarkups;
using Message = Telegram.Bot.Types.Message;

namespace Telegram.Bot.Services;

public class UpdateHandler : IUpdateHandler
{
    private readonly ITelegramBotClient _botClient;
    private readonly ILogger<UpdateHandler> _logger;
    private static string Token;
    public UpdateHandler(ITelegramBotClient botClient, ILogger<UpdateHandler> logger)
    {
        _botClient = botClient;
        _logger = logger;
    }

    public async Task HandleUpdateAsync(ITelegramBotClient _, Update update, CancellationToken cancellationToken)
    {
        try
        {
            var handler = update switch
            {
                { Message: { } message } => BotOnMessageReceived(message, cancellationToken),
                { EditedMessage: { } message } => BotOnMessageReceived(message, cancellationToken),
                _ => UnknownUpdateHandlerAsync(update, cancellationToken)
            };

            await handler;
        }
        catch (Exception ex)
        {
            await _botClient.SendTextMessageAsync(update.Message.Chat.Id, $"Что-то пошло не так. Message: {ex.Message}");
        }
    }

    private async Task<string> GetWinrate(string champion, string sumonerName, bool enemy)
    {
        RiotService riotService = new RiotService(Token);
        var account = await riotService.GetSummonerInfoByNameAsync(Region.EUW, sumonerName);
        StatisticsService statisticsService1 = new StatisticsService();
        var data = statisticsService1.GetWinrateByChampions(account.SummonerId, enemy);
        var table = new ConsoleTable("Champion", "TotalGames", "Winrate", "Kills", "Deaths", "Assists");
        var stat = data.First(_ => _.Champion.Equals(champion, StringComparison.OrdinalIgnoreCase));
        string text = $"Champion: {stat.Champion} \r\nTotal games: {stat.TotalGames}\r\nWinrate: {Math.Round((float)stat.Win / stat.TotalGames * 100, 2)}%\r\nKills: {Math.Round(stat.Kills / stat.TotalGames, 2)}\r\nDeaths: {Math.Round(stat.Deaths / stat.TotalGames, 2)}\r\nAssists: {Math.Round(stat.Assists / stat.TotalGames, 2)}";

        return text;
    }

    private async Task RegisterUser(long tgId, string summonerName)
    {
        TgService tgService = new TgService();

        await tgService.AddSummonerNameByTgId(summonerName, tgId);
        await _botClient.SendTextMessageAsync(tgId, "Готово", ParseMode.Markdown);
    }

    private async Task BotOnMessageReceived(Types.Message message, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Receive message type: {MessageType}", message.Type);
        if (message.Text is not { } messageText)
            return;
        if (messageText.StartsWith("/help"))
        {
            await _botClient.SendTextMessageAsync(message.Chat.Id, "/add НикВИгре - запомнить твой ник\r\n /update - обновить историю игр\r\n /with ИмяГероя - винрейт с чемпионом в команде\r\n /against ИмяГероя - винрейт с чемпионом во вражеской команде", ParseMode.Markdown);

            return;
        }
        TgService tgService = new TgService();
        var user = await tgService.GetSummonerNameByTgId(message.Chat.Id);
        if (!messageText.StartsWith("/add") && string.IsNullOrEmpty(user))
        {
            await _botClient.SendTextMessageAsync(message.Chat.Id, "Я вас не знаю. Воспользуйтесь командой /add ИмяВЛигеЛегенд", ParseMode.Markdown);

            return;
        }
        if (messageText.StartsWith("/update"))
        {
            await _botClient.SendTextMessageAsync(message.Chat.Id, "Запустил обновление истории матчей", ParseMode.Markdown);
            new Task(async () =>
            {
                RiotService riotService = new RiotService(Token);
                var account = await riotService.GetSummonerInfoByNameAsync(Region.EUW, user);
                await riotService.SaveMatchHistory(account);
                await _botClient.SendTextMessageAsync(message.Chat.Id, "Обновление завершено", ParseMode.Markdown);
            }).Start();

            return;
        }
        if (messageText.StartsWith("/token"))
        {
            await _botClient.SendTextMessageAsync(message.Chat.Id, "Токен обновлен", ParseMode.Markdown);
            Token = messageText.Split(' ')[1];

            return;
        }
        var parts = messageText.Split(' ');
        if (parts.Length != 2)
        {
            await _botClient.SendTextMessageAsync(message.Chat.Id, "Некорректная команда", ParseMode.Markdown);
            return;
        }
        var champion = messageText.Split(' ')[1];
        var action = messageText.Split(' ')[0] switch
        {
            "/against" => _botClient.SendTextMessageAsync(message.Chat.Id, await GetWinrate(champion, user, false), ParseMode.Markdown),
            "/with" => _botClient.SendTextMessageAsync(message.Chat.Id, await GetWinrate(champion, user, true), ParseMode.Markdown),
            "/add" => RegisterUser(message.Chat.Id, champion),
        };
        await action;

    }

#pragma warning disable IDE0060 // Remove unused parameter
#pragma warning disable RCS1163 // Unused parameter.
    private Task UnknownUpdateHandlerAsync(Update update, CancellationToken cancellationToken)
#pragma warning restore RCS1163 // Unused parameter.
#pragma warning restore IDE0060 // Remove unused parameter
    {
        _logger.LogInformation("Unknown update type: {UpdateType}", update.Type);
        return Task.CompletedTask;
    }

    public async Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        var ErrorMessage = exception switch
        {
            ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };

        _logger.LogInformation("HandleError: {ErrorMessage}", ErrorMessage);

        // Cooldown in case of network connection error
        if (exception is RequestException)
            await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
    }
}
