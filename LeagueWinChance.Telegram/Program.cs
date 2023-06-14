using LeagueWinChance.Telegram;
using Telegram.Bot;
using Telegram.Bot.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<BotConfiguration>(
            builder.Configuration.GetSection(BotConfiguration.Configuration));

builder.Services.AddHttpClient("telegram_bot_client")
                .AddTypedClient<ITelegramBotClient>((httpClient, sp) =>
                {
                    BotConfiguration? botConfig = sp.GetConfiguration<BotConfiguration>();
                    TelegramBotClientOptions options = new(botConfig.BotToken);
                    return new TelegramBotClient(options, httpClient);
                });

builder.Services.AddScoped<UpdateHandler>();
builder.Services.AddScoped<ReceiverService>();
builder.Services.AddHostedService<PollingService>();

var app = builder.Build();

app.Run();
